using MediatR;
using SkillExtraction.Application.Commands.Auth.Common;
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
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthTokenDto> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        // Check if username already exists
        if (await _userRepository.ExistsAsync(request.Username, cancellationToken))
        {
            throw new InvalidOperationException($"Username '{request.Username}' is already taken.");
        }

        // Hash password
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // Create user
        var user = new User(request.Username, passwordHash);

        // Save to repository
        await _userRepository.AddAsync(user, cancellationToken);

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
