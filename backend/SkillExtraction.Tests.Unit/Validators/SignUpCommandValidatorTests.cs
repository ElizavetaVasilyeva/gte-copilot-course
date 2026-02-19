using FluentAssertions;
using SkillExtraction.Application.Commands.Auth.SignUp;

namespace SkillExtraction.Tests.Unit.Validators;

public class SignUpCommandValidatorTests
{
    private readonly SignUpCommandValidator _validator;

    public SignUpCommandValidatorTests()
    {
        _validator = new SignUpCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new SignUpCommand
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
        var command = new SignUpCommand
        {
            Username = username!,
            Password = "TestPass123!"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SignUpCommand.Username));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_EmptyPassword_FailsValidation(string? password)
    {
        // Arrange
        var command = new SignUpCommand
        {
            Username = "testuser",
            Password = password!
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SignUpCommand.Password));
    }

    [Theory]
    [InlineData("short")]
    [InlineData("12345")]
    public void Validate_ShortPassword_FailsValidation(string password)
    {
        // Arrange
        var command = new SignUpCommand
        {
            Username = "testuser",
            Password = password
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == nameof(SignUpCommand.Password) && 
            e.ErrorMessage.Contains("6"));
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("x")]
    public void Validate_ShortUsername_FailsValidation(string username)
    {
        // Arrange
        var command = new SignUpCommand
        {
            Username = username,
            Password = "TestPass123!"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == nameof(SignUpCommand.Username) && 
            e.ErrorMessage.Contains("3"));
    }

    [Fact]
    public void Validate_LongUsername_FailsValidation()
    {
        // Arrange
        var command = new SignUpCommand
        {
            Username = new string('a', 51), // 51 characters
            Password = "TestPass123!"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == nameof(SignUpCommand.Username) && 
            e.ErrorMessage.Contains("50"));
    }

    [Theory]
    [InlineData("validuser")]
    [InlineData("user123")]
    [InlineData("test_user")]
    [InlineData("test-user")]
    public void Validate_ValidUsernames_PassValidation(string username)
    {
        // Arrange
        var command = new SignUpCommand
        {
            Username = username,
            Password = "TestPass123!"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("Password123!")]
    [InlineData("MySecurePass1!")]
    [InlineData("Test1234!@#$")]
    public void Validate_ValidPasswords_PassValidation(string password)
    {
        // Arrange
        var command = new SignUpCommand
        {
            Username = "testuser",
            Password = password
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
