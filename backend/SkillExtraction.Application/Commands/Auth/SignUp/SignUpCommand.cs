using MediatR;
using SkillExtraction.Application.Commands.Auth.Common;

namespace SkillExtraction.Application.Commands.Auth.SignUp;

/// <summary>
/// Command to register a new user.
/// </summary>
public class SignUpCommand : IRequest<AuthTokenDto>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
