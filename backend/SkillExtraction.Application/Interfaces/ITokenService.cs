namespace SkillExtraction.Application.Interfaces;

/// <summary>
/// Interface for JWT token generation.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="username">The username</param>
    /// <returns>A signed JWT token string</returns>
    string GenerateToken(Guid userId, string username);

    /// <summary>
    /// Gets the token expiration duration in seconds.
    /// </summary>
    /// <returns>Expiration duration in seconds</returns>
    int GetTokenExpirationInSeconds();
}
