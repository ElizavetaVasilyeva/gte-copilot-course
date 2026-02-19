using System.Collections.Concurrent;
using SkillExtraction.Application.Interfaces;
using SkillExtraction.Domain.Entities;

namespace SkillExtraction.Infrastructure.Persistence;

/// <summary>
/// In-memory implementation of IUserRepository for MVP.
/// </summary>
public class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<Guid, User> _users = new();
    private readonly ConcurrentDictionary<string, Guid> _usernameIndex = new();

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _users.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }

    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        if (_usernameIndex.TryGetValue(username.ToLowerInvariant(), out var userId))
        {
            return GetByIdAsync(userId, cancellationToken);
        }
        return Task.FromResult<User?>(null);
    }

    public Task<bool> ExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        var exists = _usernameIndex.ContainsKey(username.ToLowerInvariant());
        return Task.FromResult(exists);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        if (!_users.TryAdd(user.Id, user))
        {
            throw new InvalidOperationException($"User with ID '{user.Id}' already exists.");
        }

        if (!_usernameIndex.TryAdd(user.Username.ToLowerInvariant(), user.Id))
        {
            _users.TryRemove(user.Id, out _);
            throw new InvalidOperationException($"Username '{user.Username}' already exists.");
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        if (!_users.ContainsKey(user.Id))
        {
            throw new InvalidOperationException($"User with ID '{user.Id}' not found.");
        }

        _users[user.Id] = user;
        return Task.CompletedTask;
    }
}
