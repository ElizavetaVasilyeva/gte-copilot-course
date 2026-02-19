namespace SkillExtraction.Application.Commands.Auth.Common;

/// <summary>
/// Auth token response DTO.
/// </summary>
public class AuthTokenDto
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public int ExpiresInSeconds { get; set; }
}
