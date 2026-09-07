using Api.Data;
using Api.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken ) =>
        context.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Id == id, cancellationToken);

    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken) =>
        context.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Username == username, cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken ) =>
        context.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Email == email, cancellationToken);

    public async Task<IReadOnlyList<User>> GetUsersAsync(CancellationToken cancellationToken) =>
        await context.Users.AsNoTracking()
            .Where(user => user.IsActive)
            .OrderBy(user => user.FullName)
            .ToListAsync(cancellationToken);

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken)
    {
        context.Users.Add(user); // It is discouraged to use AddAsync() as per Microsoft documentation, https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext.addasync?view=efcore-10.0
        await context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(User user, CancellationToken cancellationToken)
    {
        context.Users.Remove(user);
        await context.SaveChangesAsync(cancellationToken);
    }
}