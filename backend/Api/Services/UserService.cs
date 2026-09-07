using Api.Models;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;

namespace Api.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken) =>
        userRepository.GetUserByIdAsync(id, cancellationToken);

    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken) =>
        userRepository.GetByUsernameAsync(username, cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        userRepository.GetByEmailAsync(email, cancellationToken);

    public async Task<IReadOnlyList<UserLookupResponse>> GetUsersAsync(CancellationToken cancellationToken)
    {
        var users = await userRepository.GetUsersAsync(cancellationToken);

        return users.Select(user => new UserLookupResponse
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email,
            IsActive = user.IsActive,
            Role = user.Role
        }).ToList();
    }

    public Task<User> CreateAsync(User user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (string.IsNullOrWhiteSpace(user.Email))
        {
            throw new ArgumentException("A user email is required.", nameof(user));
        }

        if (string.IsNullOrWhiteSpace(user.FullName))
        {
            throw new ArgumentException("A user full name is required.", nameof(user));
        }

        user.Email = user.Email.Trim();
        user.FullName = user.FullName.Trim();
        user.CreatedAt = user.CreatedAt == default ? DateTimeOffset.UtcNow : user.CreatedAt;

        return userRepository.AddAsync(user, cancellationToken);
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);
        return userRepository.UpdateAsync(user, cancellationToken);
    }

    public Task DeleteAsync(User user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);
        return userRepository.DeleteAsync(user, cancellationToken);
    }
}