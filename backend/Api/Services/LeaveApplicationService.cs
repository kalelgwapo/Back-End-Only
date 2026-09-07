using Api.Models;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;

namespace Api.Services;

public class LeaveApplicationService(
    ILeaveApplicationRepository repository,
    IUserRepository userRepository,
    IConfiguration configuration) : ILeaveApplicationService
{
    public async Task<LeaveApplicationResponse?> GetByApplicationIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            throw new ArgumentException("The leave application ID must be a positive integer.", nameof(id));
        }
        var application = await repository.GetByApplicationIdAsync(id, cancellationToken);
        return application is null ? null : Map(application);
    }

    public async Task<IReadOnlyList<LeaveApplicationResponse>> GetAllLeaveApplicationsAsync(
        int? page,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        ValidatePagination(page, pageSize);
        var applications = await repository.GetAllLeaveApplicationsAsync(page, pageSize, cancellationToken);
        return applications.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<LeaveApplication>> GetByApplicantIdAsync(
        int applicantId,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        if (applicantId <= 0)
        {
            throw new ArgumentException("The applicant ID must be a positive integer.", nameof(applicantId));
        }
        ValidatePagination(page, pageSize);
        return await repository.GetByApplicantIdAsync(applicantId, page, pageSize, cancellationToken);
    }

    public async Task<IReadOnlyList<LeaveApplication>> GetByManagerIdAsync(
        int managerId,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        if (managerId <= 0)
        {
            throw new ArgumentException("The manager ID must be a positive integer.", nameof(managerId));
        }
        ValidatePagination(page, pageSize);
        return await repository.GetByManagerIdAsync(managerId, page, pageSize, cancellationToken);
    }

    public async Task<IReadOnlyList<LeaveApplication>> GetByStatusAsync(
        LeaveApplicationStatus status,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentException("The status is invalid.", nameof(status));
        }
        ValidatePagination(page, pageSize);
        return await repository.GetByStatusAsync(status, page, pageSize, cancellationToken);
    }

    public async Task<IReadOnlyList<LeaveTypeOptionResponse>> GetLeaveTypeOptionsAsync(
        CancellationToken cancellationToken) =>
        (await repository.GetLeaveTypeOptions(cancellationToken)).ToList();

    public async Task<LeaveApplicationResponse> CreateAsync(
        LeaveApplicationRequest request,
        CancellationToken cancellationToken)
    {
        var application = await BuildApplicationAsync(request, cancellationToken);
        await ValidateAsync(application, cancellationToken);
        application.Status = LeaveApplicationStatus.Pending;
        application.CreatedAt = application.CreatedAt == default ? DateTimeOffset.UtcNow : application.CreatedAt;
        var savedApplication = await repository.AddAsync(application, cancellationToken);
        var applicationWithRelatedData = await repository.GetByApplicationIdAsync(savedApplication.Id, cancellationToken) ?? throw new InvalidOperationException("The leave application was saved but could not be reloaded.");
        return Map(applicationWithRelatedData);
    }

    public async Task UpdateAsync(LeaveApplication application, CancellationToken cancellationToken)
    {
        await ValidateAsync(application, cancellationToken);
        await repository.UpdateAsync(application, cancellationToken);
    }

    public async Task DeleteAsync(LeaveApplication application, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);
        await repository.DeleteAsync(application, cancellationToken);
    }

    private async Task ValidateAsync(
        LeaveApplication application,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(application);

        if (application.ApplicantId <= 0 || application.ManagerId <= 0 || application.LeaveTypeId <= 0 ||
            application.StartDate == default || application.EndDate == default ||
            application.ReturnDate == default || application.RequestedDays <= 0 ||
            string.IsNullOrWhiteSpace(application.GeneralComments))
        {
            throw new ArgumentException("All leave request fields are required.", nameof(application));
        }

        if (application.ApplicantId == application.ManagerId)
        {
            throw new ArgumentException("Applicant and manager must be distinct users.", nameof(application));
        }

        if (application.EndDate < application.StartDate)
        {
            throw new ArgumentException("The end date cannot be before the start date.", nameof(application));
        }

        if (application.ReturnDate <= application.EndDate)
        {
            throw new ArgumentException("The return date must be after the end date.", nameof(application));
        }

        var holidays = GetPublicHolidays();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (!IsWorkingDay(application.StartDate, holidays))
        {
            throw new ArgumentException("The start date must be a working day and not a public holiday.", nameof(application));
        }

        if (application.StartDate < today)
        {
            throw new ArgumentException("The start date cannot be in the past.", nameof(application));
        }

        if (CountWorkingDays(today.AddDays(1), application.StartDate, holidays) < 2)
        {
            throw new ArgumentException("The start date must be at least two working days after submission.", nameof(application));
        }

        var nextWorkingDay = application.EndDate.AddDays(1);
        while (!IsWorkingDay(nextWorkingDay, holidays))
        {
            nextWorkingDay = nextWorkingDay.AddDays(1);
        }

        if (application.ReturnDate != nextWorkingDay)
        {
            throw new ArgumentException("The return date must be the next valid working day after the end date.", nameof(application));
        }

        var calculatedDays = application.EndDate.DayNumber - application.StartDate.DayNumber + 1;
        if (application.RequestedDays != calculatedDays)
        {
            throw new ArgumentException("Requested days must match the selected date range.", nameof(application));
        }

        var leaveType = await repository.GetLeaveTypeByIdAsync(application.LeaveTypeId, cancellationToken);
        if (leaveType is null || !leaveType.IsActive)
        {
            throw new ArgumentException("Leave type must exist and be active.", nameof(application));
        }

        if (calculatedDays > leaveType.MaxDaysAllowed)
        {
            throw new ArgumentException("Requested days exceed the leave type limit.", nameof(application));
        }

        if (await repository.ExistsDuplicateAsync(
                application.ApplicantId,
                application.LeaveTypeId,
                application.StartDate,
                application.EndDate,
                cancellationToken))
        {
            throw new ArgumentException("An identical leave request already exists.", nameof(application));
        }

        if (await repository.HasOverlapAsync(
                application.ApplicantId,
                application.StartDate,
                application.EndDate,
                cancellationToken))
        {
            throw new ArgumentException("The requested dates overlap existing pending or approved leave.", nameof(application));
        }
    }

    private async Task<LeaveApplication> BuildApplicationAsync(
        LeaveApplicationRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var applicant = await userRepository.GetUserByIdAsync(request.ApplicantId, cancellationToken);
        var manager = await userRepository.GetUserByIdAsync(request.ManagerId, cancellationToken);
        var leaveType = await repository.GetLeaveTypeByIdAsync(request.LeaveTypeId, cancellationToken);

        if (applicant is null || !applicant.IsActive || manager is null || !manager.IsActive)
        {
            throw new ArgumentException("Applicant and manager must be active users.", nameof(request));
        }

        if (leaveType is null || !leaveType.IsActive)
        {
            throw new ArgumentException("Leave type must be active.", nameof(request));
        }

        return new LeaveApplication
        {
            ApplicantId = request.ApplicantId,
            ManagerId = request.ManagerId,
            LeaveTypeId = request.LeaveTypeId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            ReturnDate = request.ReturnDate,
            RequestedDays = request.RequestedDays,
            GeneralComments = request.GeneralComments?.Trim()
        };
    }

    private HashSet<DateOnly> GetPublicHolidays() =>
        configuration.GetSection("PublicHolidays").GetChildren()
            .Select(value => DateOnly.TryParse(value.Value, out var holiday) ? holiday : (DateOnly?)null)
            .Where(holiday => holiday.HasValue)
            .Select(holiday => holiday!.Value)
            .ToHashSet();

    private static bool IsWorkingDay(DateOnly date, HashSet<DateOnly> holidays) =>
        date.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday && !holidays.Contains(date); // Possible edge case where holidays only populated dates for the current calendar year and the leave application ends on December 31st, any non-weekend days in January will be treated as working days (unless added to holidays). This won't cause a loop issue, but it could produce incorrect leave calculations across new year boundaries.

    private static int CountWorkingDays(DateOnly start, DateOnly end, HashSet<DateOnly> holidays)
    {
        var count = 0;
        for (var date = start; date <= end; date = date.AddDays(1))
        {
            if (IsWorkingDay(date, holidays))
            {
                count++;
            }
        }

        return count;
    }

    private static LeaveApplicationResponse Map(LeaveApplication application) => new()
    {
        Id = application.Id,
        ApplicantId = application.ApplicantId,
        Applicant = application.Applicant.FullName,
        ManagerId = application.ManagerId,
        Manager = application.Manager.FullName,
        LeaveTypeId = application.LeaveTypeId,
        LeaveType = application.LeaveType.Name,
        StartDate = application.StartDate,
        EndDate = application.EndDate,
        ReturnDate = application.ReturnDate,
        RequestedDays = application.RequestedDays,
        GeneralComments = application.GeneralComments,
        Status = application.Status.ToString(),
        CreatedAt = application.CreatedAt
    };

    private static void ValidatePagination(int? page, int? pageSize)
    {
        if (!page.HasValue && !pageSize.HasValue)
        {
            return;
        }

        if (!page.HasValue || !pageSize.HasValue || page.Value < 1 || pageSize.Value < 1)
        {
            throw new ArgumentException("page and pageSize must be positive values provided together.");
        }
    }
}