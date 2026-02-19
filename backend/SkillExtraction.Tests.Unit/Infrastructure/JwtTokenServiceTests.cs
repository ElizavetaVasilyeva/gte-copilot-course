using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using SkillExtraction.Infrastructure.Security;

namespace SkillExtraction.Tests.Unit.Infrastructure;

public class JwtTokenServiceTests
{
    private readonly IConfiguration _configuration;
    private readonly JwtTokenService _tokenService;

    public JwtTokenServiceTests()
    {
        var configValues = new Dictionary<string, string?>
        {
            { "JwtSettings:Secret", "ThisIsAVerySecretKeyForJwtTokenGeneration123456" },
            { "JwtSettings:Issuer", "SkillExtractionAPI" },
            { "JwtSettings:Audience", "SkillExtractionClient" },
            { "JwtSettings:ExpirationInMinutes", "60" }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues!)
            .Build();

        _tokenService = new JwtTokenService(_configuration);
    }

    [Fact]
    public void GenerateToken_ValidInputs_ReturnsJwtToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var username = "testuser";

        // Act
        var token = _tokenService.GenerateToken(userId, username);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Split('.').Should().HaveCount(3); // JWT has 3 parts: header.payload.signature
    }

    [Fact]
    public void GenerateToken_ValidInputs_ContainsCorrectClaims()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var username = "testuser";

        // Act
        var token = _tokenService.GenerateToken(userId, username);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.UniqueName && c.Value == username);
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == username);
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == userId.ToString());
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);
    }

    [Fact]
    public void GenerateToken_ValidInputs_HasCorrectIssuerAndAudience()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var username = "testuser";

        // Act
        var token = _tokenService.GenerateToken(userId, username);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.Issuer.Should().Be("SkillExtractionAPI");
        jwtToken.Audiences.Should().Contain("SkillExtractionClient");
    }

    [Fact]
    public void GenerateToken_ValidInputs_HasCorrectExpiration()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var username = "testuser";
        var beforeGeneration = DateTime.UtcNow;

        // Act
        var token = _tokenService.GenerateToken(userId, username);
        var afterGeneration = DateTime.UtcNow;

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.ValidTo.Should().BeAfter(beforeGeneration.AddMinutes(59));
        jwtToken.ValidTo.Should().BeBefore(afterGeneration.AddMinutes(61));
    }

    [Fact]
    public void GenerateToken_DifferentUsers_GeneratesDifferentTokens()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var username1 = "user1";
        var username2 = "user2";

        // Act
        var token1 = _tokenService.GenerateToken(userId1, username1);
        var token2 = _tokenService.GenerateToken(userId2, username2);

        // Assert
        token1.Should().NotBe(token2);
    }

    [Fact]
    public void GenerateToken_SameUserTwice_GeneratesDifferentTokens()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var username = "testuser";

        // Act
        var token1 = _tokenService.GenerateToken(userId, username);
        Thread.Sleep(10); // Ensure different JTI (timestamp-based)
        var token2 = _tokenService.GenerateToken(userId, username);

        // Assert
        token1.Should().NotBe(token2); // Different JTI claim
    }

    [Fact]
    public void GetTokenExpirationInSeconds_ReturnsCorrectValue()
    {
        // Act
        var expirationSeconds = _tokenService.GetTokenExpirationInSeconds();

        // Assert
        expirationSeconds.Should().Be(3600); // 60 minutes * 60 seconds
    }

    [Fact]
    public void GenerateToken_MissingSecretConfiguration_ThrowsException()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            { "JwtSettings:Issuer", "SkillExtractionAPI" },
            { "JwtSettings:Audience", "SkillExtractionClient" },
            { "JwtSettings:ExpirationInMinutes", "60" }
            // Missing Secret
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        var tokenService = new JwtTokenService(configuration);
        var userId = Guid.NewGuid();

        // Act
        var act = () => tokenService.GenerateToken(userId, "testuser");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Secret*");
    }

    [Theory]
    [InlineData("testuser")]
    [InlineData("admin")]
    [InlineData("user@example.com")]
    [InlineData("test-user_123")]
    public void GenerateToken_VariousUsernames_GeneratesValidTokens(string username)
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var token = _tokenService.GenerateToken(userId, username);

        // Assert
        token.Should().NotBeNullOrEmpty();
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == username);
    }

    [Fact]
    public void GenerateToken_CustomExpiration_RespectsConfiguration()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            { "JwtSettings:Secret", "ThisIsAVerySecretKeyForJwtTokenGeneration123456" },
            { "JwtSettings:Issuer", "SkillExtractionAPI" },
            { "JwtSettings:Audience", "SkillExtractionClient" },
            { "JwtSettings:ExpirationInMinutes", "30" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        var tokenService = new JwtTokenService(configuration);
        var userId = Guid.NewGuid();

        // Act
        var expirationSeconds = tokenService.GetTokenExpirationInSeconds();
        var token = tokenService.GenerateToken(userId, "testuser");

        // Assert
        expirationSeconds.Should().Be(1800); // 30 minutes * 60 seconds

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        jwtToken.ValidTo.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(30), TimeSpan.FromSeconds(5));
    }
}
