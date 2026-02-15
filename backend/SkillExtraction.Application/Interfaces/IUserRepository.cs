using SkillExtraction.Domain.Entities;

namespace SkillExtraction.Application.Interfaces;

/// <summary>
/// Repository interface for User entity operations.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user if found, otherwise null</returns>
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by their username.
    /// </summary>
    /// <param name="username">The username to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user if found, otherwise null</returns>
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user with the specified username exists.
    /// </summary>
    /// <param name="username">The username to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the username exists, otherwise false</returns>
    Task<bool> ExistsAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new user to the repository.
    /// </summary>
    /// <param name="user">The user to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user in the repository.
    /// </summary>
    /// <param name="user">The user with updated information</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
}
