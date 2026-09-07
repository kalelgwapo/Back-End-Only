namespace Api.Models;

public class LeaveTypeOptionResponse
{
    public string Value { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public int MaxDaysPerYear { get; init; }
}