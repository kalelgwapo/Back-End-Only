namespace Api.Models;

public class LeaveApplicationRequest
{
    public int ApplicantId { get; set; }
    public int ManagerId { get; set; }
    public int LeaveTypeId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateOnly ReturnDate { get; set; }
    public int RequestedDays { get; set; }
    public string? GeneralComments { get; set; }
}