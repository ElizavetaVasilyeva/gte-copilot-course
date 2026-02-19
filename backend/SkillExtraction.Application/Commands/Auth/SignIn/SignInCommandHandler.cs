using MediatR;
using SkillExtraction.Application.Commands.Auth.Common;
using SkillExtraction.Application.Exceptions;
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
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    /// <summary>
    /// Handles the sign-in command to authenticate an existing user.
    /// </summary>
    /// <param name="request">The sign-in command containing username and password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An authentication token for the authenticated user</returns>
    public async Task<AuthTokenDto> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        // Find user by username
        var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken).ConfigureAwait(false);
        
        if (user == null)
        {
            throw new InvalidCredentialsException();
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException();
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
