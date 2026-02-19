using MediatR;
using SkillExtraction.Application.Commands.Auth.Common;

namespace SkillExtraction.Application.Commands.Auth.SignIn;

/// <summary>
/// Command to sign in an existing user.
/// </summary>
public class SignInCommand : IRequest<AuthTokenDto>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
