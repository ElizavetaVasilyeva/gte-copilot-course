using Microsoft.EntityFrameworkCore;
using SkillExtraction.Application.Interfaces;
using SkillExtraction.Domain.Entities;

namespace SkillExtraction.Infrastructure.Persistence;

/// <summary>
/// SQLite-backed implementation of user repository using Entity Framework Core.
/// </summary>
public class SqliteUserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public SqliteUserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> ExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Username == username, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken).ConfigureAwait(false);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
