using MediatR;
using SkillExtraction.Application.Commands.Auth.Common;
using SkillExtraction.Application.Exceptions;
using SkillExtraction.Application.Interfaces;
using SkillExtraction.Domain.Entities;

namespace SkillExtraction.Application.Commands.Auth.SignUp;

/// <summary>
/// Handler for SignUpCommand.
/// </summary>
public class SignUpCommandHandler : IRequestHandler<SignUpCommand, AuthTokenDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public SignUpCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    /// <summary>
    /// Handles the sign-up command to create a new user account.
    /// </summary>
    /// <param name="request">The sign-up command containing username and password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An authentication token for the newly created user</returns>
    public async Task<AuthTokenDto> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        // Check if username already exists
        if (await _userRepository.ExistsAsync(request.Username, cancellationToken).ConfigureAwait(false))
        {
            throw new UserAlreadyExistsException(request.Username);
        }

        // Hash password
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // Create user
        var user = new User(request.Username, passwordHash);

        // Save to repository
        await _userRepository.AddAsync(user, cancellationToken).ConfigureAwait(false);

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
