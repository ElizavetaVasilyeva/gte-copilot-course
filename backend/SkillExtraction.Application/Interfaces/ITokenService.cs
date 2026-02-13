namespace SkillExtraction.Application.Interfaces;

/// <summary>
/// Interface for JWT token generation.
/// </summary>
public interface ITokenService
{
    string GenerateToken(Guid userId, string username);
    int GetTokenExpirationInSeconds();
}
