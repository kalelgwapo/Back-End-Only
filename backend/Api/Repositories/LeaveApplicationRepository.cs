using Api.Data;
using Api.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories;

public class LeaveApplicationRepository(ApplicationDbContext context) : ILeaveApplicationRepository
{
    public Task<LeaveApplication?> GetByApplicationIdAsync(int id, CancellationToken cancellationToken) =>
        QueryWithRelatedData().FirstOrDefaultAsync(application => application.Id == id, cancellationToken);

    public async Task<IEnumerable<LeaveTypeOptionResponse>> GetLeaveTypeOptions(
        CancellationToken cancellationToken)
    {
        return await context.LeaveTypes
            .AsNoTracking()
            .Where(leaveType => leaveType.IsActive)
            .OrderBy(leaveType => leaveType.Id)
            .Select(leaveType => new LeaveTypeOptionResponse
            {
                Value = leaveType.Name,
                Label = leaveType.Name,
                MaxDaysPerYear = leaveType.MaxDaysAllowed
            })
            .ToListAsync(cancellationToken);
    }

    public Task<LeaveType?> GetLeaveTypeByIdAsync(int id, CancellationToken cancellationToken) =>
        context.LeaveTypes.AsNoTracking()
            .FirstOrDefaultAsync(leaveType => leaveType.Id == id, cancellationToken);

    public Task<bool> HasOverlapAsync(
        int applicantId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken) =>
        context.LeaveApplications.AsNoTracking().AnyAsync(application =>
            application.ApplicantId == applicantId &&
            (application.Status == LeaveApplicationStatus.Pending || application.Status == LeaveApplicationStatus.Approved) &&
            application.StartDate <= endDate &&
            application.EndDate >= startDate,
            cancellationToken);

    public Task<bool> ExistsDuplicateAsync(
        int applicantId,
        int leaveTypeId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken) =>
        context.LeaveApplications.AsNoTracking().AnyAsync(application =>
            application.ApplicantId == applicantId &&
            application.LeaveTypeId == leaveTypeId &&
            application.StartDate == startDate &&
            application.EndDate == endDate,
            cancellationToken);
    public async Task<IReadOnlyList<LeaveApplication>> GetAllLeaveApplicationsAsync(
        int? page,
        int? pageSize,
        CancellationToken cancellationToken) =>
        await ApplyPagination(QueryWithRelatedData()
            .OrderByDescending(application => application.CreatedAt)
            .ThenByDescending(application => application.Id), page, pageSize)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<LeaveApplication>> GetByApplicantIdAsync(
        int applicantId,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken) =>
        await ApplyPagination(QueryWithRelatedData()
            .Where(application => application.ApplicantId == applicantId)
            .OrderByDescending(application => application.CreatedAt)
            .ThenByDescending(application => application.Id), page, pageSize)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<LeaveApplication>> GetByManagerIdAsync(
        int managerId,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken) =>
        await ApplyPagination(QueryWithRelatedData()
            .Where(application => application.ManagerId == managerId)
            .OrderByDescending(application => application.CreatedAt)
            .ThenByDescending(application => application.Id), page, pageSize)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<LeaveApplication>> GetByStatusAsync(
        LeaveApplicationStatus status,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken) =>
        await ApplyPagination(QueryWithRelatedData()
            .Where(application => application.Status == status)
            .OrderByDescending(application => application.CreatedAt)
            .ThenByDescending(application => application.Id), page, pageSize)
            .ToListAsync(cancellationToken);

    public async Task<LeaveApplication> AddAsync(
        LeaveApplication application,
        CancellationToken cancellationToken)
    {
        context.LeaveApplications.Add(application); // It is discouraged to use AddAsync() as per Microsoft documentation, https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext.addasync?view=efcore-10.0
        await context.SaveChangesAsync(cancellationToken);
        return application;
    }

    public async Task UpdateAsync(LeaveApplication application, CancellationToken cancellationToken)
    {
        context.LeaveApplications.Update(application);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(LeaveApplication application, CancellationToken cancellationToken)
    {
        context.LeaveApplications.Remove(application);
        await context.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<LeaveApplication> QueryWithRelatedData() =>
        context.LeaveApplications
            .AsNoTracking()
            .Include(application => application.Applicant)
            .Include(application => application.Manager)
            .Include(application => application.LeaveType);

    private static IQueryable<LeaveApplication> ApplyPagination(
        IQueryable<LeaveApplication> query,
        int? page,
        int? pageSize)
    {
        if (!page.HasValue && !pageSize.HasValue)
        {
            return query;
        }

        if (!page.HasValue || !pageSize.HasValue)
        {
            throw new ArgumentException("Page and pageSize must be provided together.");
        }

        return query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
    }
}