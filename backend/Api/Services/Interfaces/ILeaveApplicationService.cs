using Api.Models;

namespace Api.Services.Interfaces;

public interface ILeaveApplicationService
{
    Task<LeaveApplicationResponse?> GetByApplicationIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<LeaveApplicationResponse>> GetAllLeaveApplicationsAsync(int? page, int? pageSize, CancellationToken cancellationToken);
    Task<IReadOnlyList<LeaveApplication>> GetByApplicantIdAsync(int applicantId, int? page, int? pageSize, CancellationToken cancellationToken);
    Task<IReadOnlyList<LeaveApplication>> GetByManagerIdAsync(int managerId, int? page, int? pageSize, CancellationToken cancellationToken);
    Task<IReadOnlyList<LeaveApplication>> GetByStatusAsync(LeaveApplicationStatus status, int? page, int? pageSize, CancellationToken cancellationToken);
    Task<IReadOnlyList<LeaveTypeOptionResponse>> GetLeaveTypeOptionsAsync(CancellationToken cancellationToken);
    Task<LeaveApplicationResponse> CreateAsync(LeaveApplicationRequest request, CancellationToken cancellationToken);
    Task UpdateAsync(LeaveApplication application, CancellationToken cancellationToken);
    Task DeleteAsync(LeaveApplication application, CancellationToken cancellationToken);
}