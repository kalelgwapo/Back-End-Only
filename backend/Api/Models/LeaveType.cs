namespace Api.Models;

public class LeaveType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MaxDaysAllowed { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<LeaveApplication> Applications { get; set; } = [];
}