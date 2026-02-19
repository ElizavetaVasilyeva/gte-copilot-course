using FluentAssertions;
using SkillExtraction.Application.Commands.Auth.SignIn;

namespace SkillExtraction.Tests.Unit.Validators;

public class SignInCommandValidatorTests
{
    private readonly SignInCommandValidator _validator;

    public SignInCommandValidatorTests()
    {
        _validator = new SignInCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new SignInCommand
        {
            Username = "testuser",
            Password = "TestPass123!"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_EmptyUsername_FailsValidation(string? username)
    {
        // Arrange
        var command = new SignInCommand
        {
            Username = username!,
            Password = "TestPass123!"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SignInCommand.Username));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_EmptyPassword_FailsValidation(string? password)
    {
        // Arrange
        var command = new SignInCommand
        {
            Username = "testuser",
            Password = password!
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SignInCommand.Password));
    }

    [Theory]
    [InlineData("testuser", "password")]
    [InlineData("admin", "admin123")]
    [InlineData("user@example.com", "SecurePass1!")]
    public void Validate_ValidCredentials_PassValidation(string username, string password)
    {
        // Arrange
        var command = new SignInCommand
        {
            Username = username,
            Password = password
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_BothFieldsEmpty_ReturnsMultipleErrors()
    {
        // Arrange
        var command = new SignInCommand
        {
            Username = string.Empty,
            Password = string.Empty
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterOrEqualTo(2);
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SignInCommand.Username));
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SignInCommand.Password));
    }
}
