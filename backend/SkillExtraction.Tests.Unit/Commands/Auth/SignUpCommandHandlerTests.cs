using FluentAssertions;
using NSubstitute;
using SkillExtraction.Application.Commands.Auth.SignUp;
using SkillExtraction.Application.Exceptions;
using SkillExtraction.Application.Interfaces;
using SkillExtraction.Domain.Entities;

namespace SkillExtraction.Tests.Unit.Commands.Auth;

public class SignUpCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly SignUpCommandHandler _handler;

    public SignUpCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _tokenService = Substitute.For<ITokenService>();
        _handler = new SignUpCommandHandler(_userRepository, _passwordHasher, _tokenService);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsAuthTokenDto()
    {
        // Arrange
        var command = new SignUpCommand
        {
            Username = "testuser",
            Password = "TestPass123!"
        };

        var hashedPassword = "hashed_password_123";
        var jwtToken = "jwt_token_abc";
        var expirationSeconds = 3600;

        _userRepository.ExistsAsync(command.Username, Arg.Any<CancellationToken>())
            .Returns(false);
        _passwordHasher.HashPassword(command.Password)
            .Returns(hashedPassword);
        _tokenService.GenerateToken(Arg.Any<Guid>(), command.Username)
            .Returns(jwtToken);
        _tokenService.GetTokenExpirationInSeconds()
            .Returns(expirationSeconds);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be(jwtToken);
        result.Username.Should().Be(command.Username);
        result.ExpiresInSeconds.Should().Be(expirationSeconds);

        await _userRepository.Received(1).ExistsAsync(command.Username, Arg.Any<CancellationToken>());
        _passwordHasher.Received(1).HashPassword(command.Password);
        _tokenService.Received(1).GenerateToken(Arg.Any<Guid>(), command.Username);
        _tokenService.Received(1).GetTokenExpirationInSeconds();
        await _userRepository.Received(1).AddAsync(Arg.Is<User>(u => 
            u.Username == command.Username && 
            u.PasswordHash == hashedPassword), 
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ExistingUsername_ThrowsInvalidOperationException()
    {
        // Arrange
        var command = new SignUpCommand
        {
            Username = "existinguser",
            Password = "TestPass123!"
        };

        _userRepository.ExistsAsync(command.Username, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UserAlreadyExistsException>()
            .WithMessage("*already taken*");

        await _userRepository.Received(1).ExistsAsync(command.Username, Arg.Any<CancellationToken>());
        _passwordHasher.DidNotReceive().HashPassword(Arg.Any<string>());
        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Handle_InvalidUsername_ThrowsArgumentException(string? username)
    {
        // Arrange
        var command = new SignUpCommand
        {
            Username = username!,
            Password = "TestPass123!"
        };

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Note: This test assumes validation happens before the handler
        // In real implementation, FluentValidation would catch this
        // But we test the domain entity validation
        if (string.IsNullOrWhiteSpace(username))
        {
            await act.Should().ThrowAsync<ArgumentException>();
        }
    }
}
