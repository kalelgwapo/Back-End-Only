using Api.Models;

namespace Api.Repositories.Interfaces;

public interface ILeaveApplicationRepository
{
    Task<LeaveApplication?> GetByApplicationIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<LeaveTypeOptionResponse>> GetLeaveTypeOptions(CancellationToken cancellationToken);
    Task<LeaveType?> GetLeaveTypeByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> HasOverlapAsync(int applicantId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);
    Task<bool> ExistsDuplicateAsync(int applicantId, int leaveTypeId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);
    Task<IReadOnlyList<LeaveApplication>> GetAllLeaveApplicationsAsync(int? page, int? pageSize, CancellationToken cancellationToken);
    Task<IReadOnlyList<LeaveApplication>> GetByApplicantIdAsync(int applicantId, int? page, int? pageSize, CancellationToken cancellationToken);
    Task<IReadOnlyList<LeaveApplication>> GetByManagerIdAsync(int managerId, int? page, int? pageSize, CancellationToken cancellationToken);
    Task<IReadOnlyList<LeaveApplication>> GetByStatusAsync(LeaveApplicationStatus status, int? page, int? pageSize, CancellationToken cancellationToken);
    Task<LeaveApplication> AddAsync(LeaveApplication application, CancellationToken cancellationToken);
    Task UpdateAsync(LeaveApplication application, CancellationToken cancellationToken);
    Task DeleteAsync(LeaveApplication application, CancellationToken cancellationToken);
}