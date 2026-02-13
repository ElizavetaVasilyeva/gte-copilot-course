using MediatR;
using SkillExtraction.Application.Commands.Auth.Common;
using SkillExtraction.Application.Interfaces;

namespace SkillExtraction.Application.Commands.Auth.SignIn;

/// <summary>
/// Handler for SignInCommand.
/// </summary>
public class SignInCommandHandler : IRequestHandler<SignInCommand, AuthTokenDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public SignInCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthTokenDto> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        // Find user by username
        var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
        
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        // Generate JWT token
        var token = _tokenService.GenerateToken(user.Id, user.Username);
        var expiresInSeconds = _tokenService.GetTokenExpirationInSeconds();

        return new AuthTokenDto
        {
            Token = token,
            Username = user.Username,
            ExpiresInSeconds = expiresInSeconds
        };
    }
}
