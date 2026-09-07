namespace Api.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string Role { get; set; } = "Employee";
    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<LeaveApplication> Applications { get; set; } = [];
    public ICollection<LeaveApplication> ManagedApplications { get; set; } = [];
}