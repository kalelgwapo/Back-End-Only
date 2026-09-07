using Api.Models;

namespace Api.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<IReadOnlyList<User>> GetUsersAsync(CancellationToken cancellationToken);
    Task<User> AddAsync(User user, CancellationToken cancellationToken);
    Task UpdateAsync(User user, CancellationToken cancellationToken);
    Task DeleteAsync(User user, CancellationToken cancellationToken);
}