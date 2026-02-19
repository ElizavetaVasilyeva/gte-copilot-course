using FluentAssertions;
using NSubstitute;
using SkillExtraction.Application.Commands.Auth.SignIn;
using SkillExtraction.Application.Exceptions;
using SkillExtraction.Application.Interfaces;
using SkillExtraction.Domain.Entities;

namespace SkillExtraction.Tests.Unit.Commands.Auth;

public class SignInCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly SignInCommandHandler _handler;

    public SignInCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _tokenService = Substitute.For<ITokenService>();
        _handler = new SignInCommandHandler(_userRepository, _passwordHasher, _tokenService);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsAuthTokenDto()
    {
        // Arrange
        var command = new SignInCommand
        {
            Username = "testuser",
            Password = "TestPass123!"
        };

        var hashedPassword = "hashed_password_123";
        var user = new User(command.Username, hashedPassword);
        var jwtToken = "jwt_token_abc";
        var expirationSeconds = 3600;

        _userRepository.GetByUsernameAsync(command.Username, Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.VerifyPassword(command.Password, user.PasswordHash)
            .Returns(true);
        _tokenService.GenerateToken(user.Id, command.Username)
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

        await _userRepository.Received(1).GetByUsernameAsync(command.Username, Arg.Any<CancellationToken>());
        _passwordHasher.Received(1).VerifyPassword(command.Password, user.PasswordHash);
        _tokenService.Received(1).GenerateToken(user.Id, command.Username);
        _tokenService.Received(1).GetTokenExpirationInSeconds();
    }

    [Fact]
    public async Task Handle_NonExistentUser_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var command = new SignInCommand
        {
            Username = "nonexistentuser",
            Password = "TestPass123!"
        };

        _userRepository.GetByUsernameAsync(command.Username, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialsException>()
            .WithMessage("*username or password*");

        await _userRepository.Received(1).GetByUsernameAsync(command.Username, Arg.Any<CancellationToken>());
        _passwordHasher.DidNotReceive().VerifyPassword(Arg.Any<string>(), Arg.Any<string>());
        _tokenService.DidNotReceive().GenerateToken(Arg.Any<Guid>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_InvalidPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var command = new SignInCommand
        {
            Username = "testuser",
            Password = "WrongPassword!"
        };

        var hashedPassword = "hashed_password_123";
        var user = new User(command.Username, hashedPassword);

        _userRepository.GetByUsernameAsync(command.Username, Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.VerifyPassword(command.Password, user.PasswordHash)
            .Returns(false);
        _tokenService.GenerateToken(Arg.Any<Guid>(), Arg.Any<string>())
            .Returns("unused_token");

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialsException>()
            .WithMessage("*username or password*");

        await _userRepository.Received(1).GetByUsernameAsync(command.Username, Arg.Any<CancellationToken>());
        _passwordHasher.Received(1).VerifyPassword(command.Password, user.PasswordHash);
        _tokenService.DidNotReceive().GenerateToken(Arg.Any<Guid>(), Arg.Any<string>());
    }
}
