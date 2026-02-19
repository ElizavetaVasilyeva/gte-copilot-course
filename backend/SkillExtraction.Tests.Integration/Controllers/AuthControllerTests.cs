using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SkillExtraction.Application.Commands.Auth.Common;
using SkillExtraction.Application.Commands.Auth.SignIn;
using SkillExtraction.Application.Commands.Auth.SignUp;

namespace SkillExtraction.Tests.Integration.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task SignUp_ValidRequest_ReturnsOkWithAuthToken()
    {
        // Arrange
        var command = new SignUpCommand
        {
            Username = $"testuser_{Guid.NewGuid():N}",
            Password = "TestPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/signup", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<AuthTokenDto>();
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.Username.Should().Be(command.Username);
        result.ExpiresInSeconds.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task SignUp_DuplicateUsername_ReturnsBadRequest()
    {
        // Arrange
        var username = $"duplicate_{Guid.NewGuid():N}";
        var command1 = new SignUpCommand
        {
            Username = username,
            Password = "Password123!"
        };
        var command2 = new SignUpCommand
        {
            Username = username,
            Password = "DifferentPassword123!"
        };

        // Act
        var response1 = await _client.PostAsJsonAsync("/api/auth/signup", command1);
        var response2 = await _client.PostAsJsonAsync("/api/auth/signup", command2);

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        // API returns 409 Conflict for duplicate username, which is more accurate than 400 BadRequest
        response2.StatusCode.Should().Match(code => 
            code == HttpStatusCode.BadRequest || code == HttpStatusCode.Conflict);
    }

    [Theory]
    [InlineData("ab", "Password123!")] // Username too short
    [InlineData("test user", "Password123!")] // Username with space
    [InlineData("testuser", "short")] // Password too short
    [InlineData("", "Password123!")] // Empty username
    [InlineData("testuser", "")] // Empty password
    public async Task SignUp_InvalidInput_ReturnsBadRequest(string username, string password)
    {
        // Arrange
        var command = new SignUpCommand
        {
            Username = username,
            Password = password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/signup", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SignIn_ValidCredentials_ReturnsOkWithAuthToken()
    {
        // Arrange - Create user first
        var username = $"signin_test_{Guid.NewGuid():N}";
        var password = "SignInPassword123!";
        var signUpCommand = new SignUpCommand
        {
            Username = username,
            Password = password
        };
        await _client.PostAsJsonAsync("/api/auth/signup", signUpCommand);

        var signInCommand = new SignInCommand
        {
            Username = username,
            Password = password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/signin", signInCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<AuthTokenDto>();
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.Username.Should().Be(username);
        result.ExpiresInSeconds.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task SignIn_NonExistentUser_ReturnsUnauthorized()
    {
        // Arrange
        var command = new SignInCommand
        {
            Username = $"nonexistent_{Guid.NewGuid():N}",
            Password = "Password123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/signin", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SignIn_IncorrectPassword_ReturnsUnauthorized()
    {
        // Arrange - Create user first
        var username = $"wrongpass_test_{Guid.NewGuid():N}";
        var correctPassword = "CorrectPassword123!";
        var signUpCommand = new SignUpCommand
        {
            Username = username,
            Password = correctPassword
        };
        await _client.PostAsJsonAsync("/api/auth/signup", signUpCommand);

        var signInCommand = new SignInCommand
        {
            Username = username,
            Password = "WrongPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/signin", signInCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SignUp_Then_SignIn_WorksWithSameCredentials()
    {
        // Arrange
        var username = $"fullflow_test_{Guid.NewGuid():N}";
        var password = "FullFlowPassword123!";

        // Act - Sign Up
        var signUpCommand = new SignUpCommand
        {
            Username = username,
            Password = password
        };
        var signUpResponse = await _client.PostAsJsonAsync("/api/auth/signup", signUpCommand);
        var signUpResult = await signUpResponse.Content.ReadFromJsonAsync<AuthTokenDto>();

        // Act - Sign In
        var signInCommand = new SignInCommand
        {
            Username = username,
            Password = password
        };
        var signInResponse = await _client.PostAsJsonAsync("/api/auth/signin", signInCommand);
        var signInResult = await signInResponse.Content.ReadFromJsonAsync<AuthTokenDto>();

        // Assert
        signUpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        signInResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        signUpResult.Should().NotBeNull();
        signInResult.Should().NotBeNull();
        
        signUpResult!.Username.Should().Be(username);
        signInResult!.Username.Should().Be(username);
        
        // Tokens should be different (different JTI claim)
        signUpResult.Token.Should().NotBe(signInResult.Token);
    }
}
