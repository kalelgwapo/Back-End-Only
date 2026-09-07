namespace Api.Models;

public class LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public DateTime ExpiresAtUtc { get; init; }
    public UserLookupResponse User { get; init; } = new();
}