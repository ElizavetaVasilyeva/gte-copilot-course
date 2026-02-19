using FluentAssertions;
using SkillExtraction.Infrastructure.Security;

namespace SkillExtraction.Tests.Unit.Infrastructure;

public class PasswordHasherAdapterTests
{
    private readonly PasswordHasherAdapter _hasher;

    public PasswordHasherAdapterTests()
    {
        _hasher = new PasswordHasherAdapter();
    }

    [Fact]
    public void HashPassword_ValidPassword_ReturnsHashedString()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var hash = _hasher.HashPassword(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(password);
        hash.Length.Should().BeGreaterThan(password.Length);
    }

    [Fact]
    public void HashPassword_SamePasswordTwice_ReturnsDifferentHashes()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var hash1 = _hasher.HashPassword(password);
        var hash2 = _hasher.HashPassword(password);

        // Assert
        hash1.Should().NotBeNullOrEmpty();
        hash2.Should().NotBeNullOrEmpty();
        hash1.Should().NotBe(hash2); // Different salts
    }

    [Fact]
    public void VerifyPassword_CorrectPassword_ReturnsTrue()
    {
        // Arrange
        var password = "TestPassword123!";
        var hash = _hasher.HashPassword(password);

        // Act
        var result = _hasher.VerifyPassword(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_IncorrectPassword_ReturnsFalse()
    {
        // Arrange
        var correctPassword = "TestPassword123!";
        var wrongPassword = "WrongPassword456!";
        var hash = _hasher.HashPassword(correctPassword);

        // Act
        var result = _hasher.VerifyPassword(wrongPassword, hash);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("Password123!")]
    [InlineData("MySecurePass1!")]
    [InlineData("Test1234!@#$")]
    [InlineData("simple")]
    [InlineData("")]
    public void HashAndVerify_VariousPasswords_WorksCorrectly(string password)
    {
        // Arrange & Act
        var hash = _hasher.HashPassword(password);
        var verifyCorrect = _hasher.VerifyPassword(password, hash);
        var verifyWrong = _hasher.VerifyPassword(password + "x", hash);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        verifyCorrect.Should().BeTrue();
        verifyWrong.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_EmptyHash_ReturnsFalse()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var result = _hasher.VerifyPassword(password, string.Empty);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_InvalidHash_ThrowsOrReturnsFalse()
    {
        // Arrange
        var password = "TestPassword123!";
        var invalidHash = "not_a_valid_hash";

        // Act & Assert
        // PasswordHasher may throw FormatException for invalid base64 hashes
        // This is expected behavior as the hash format is invalid
        var act = () => _hasher.VerifyPassword(password, invalidHash);
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void HashPassword_LongPassword_HandlesCorrectly()
    {
        // Arrange
        var longPassword = new string('a', 500);

        // Act
        var hash = _hasher.HashPassword(longPassword);
        var verify = _hasher.VerifyPassword(longPassword, hash);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        verify.Should().BeTrue();
    }

    [Fact]
    public void HashPassword_PasswordWithSpecialCharacters_HandlesCorrectly()
    {
        // Arrange
        var password = "P@$$w0rd!#%&*()[]{}|\\/<>?";

        // Act
        var hash = _hasher.HashPassword(password);
        var verify = _hasher.VerifyPassword(password, hash);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        verify.Should().BeTrue();
    }

    [Fact]
    public void HashPassword_UnicodePassword_HandlesCorrectly()
    {
        // Arrange
        var password = "密码🔒Пароль";

        // Act
        var hash = _hasher.HashPassword(password);
        var verify = _hasher.VerifyPassword(password, hash);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        verify.Should().BeTrue();
    }
}
