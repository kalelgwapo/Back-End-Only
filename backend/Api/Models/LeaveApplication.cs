namespace Api.Models;

public class LeaveApplication
{
    public int Id { get; set; }
    public int ApplicantId { get; set; }
    public int ManagerId { get; set; }
    public int LeaveTypeId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateOnly ReturnDate { get; set; }
    public int RequestedDays { get; set; }
    public string? GeneralComments { get; set; }
    public LeaveApplicationStatus Status { get; set; } = LeaveApplicationStatus.Pending;
    public DateTimeOffset CreatedAt { get; set; }

    public User Applicant { get; set; } = null!;
    public User Manager { get; set; } = null!;
    public LeaveType LeaveType { get; set; } = null!;
}