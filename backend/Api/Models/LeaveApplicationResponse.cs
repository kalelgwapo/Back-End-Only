namespace Api.Models;

public class LeaveApplicationResponse
{
    public int Id { get; init; }
    public int ApplicantId { get; init; }
    public string Applicant { get; init; } = string.Empty;
    public int ManagerId { get; init; }
    public string Manager { get; init; } = string.Empty;
    public int LeaveTypeId { get; init; }
    public string LeaveType { get; init; } = string.Empty;
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public DateOnly ReturnDate { get; init; }
    public int RequestedDays { get; init; }
    public string? GeneralComments { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
}