using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace SkillExtraction.Tests.Integration;

/// <summary>
/// Custom WebApplicationFactory for integration tests.
/// Configures the application to use in-memory database for testing.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseConfiguration(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "DatabaseSettings:Provider", "InMemory" },
                { "JwtSettings:Secret", "TestSecretKeyForJwtTokenGeneration12345678901234567890" },
                { "JwtSettings:Issuer", "SkillExtractionTestAPI" },
                { "JwtSettings:Audience", "SkillExtractionTestClient" },
                { "JwtSettings:ExpirationInMinutes", "60" }
            })
            .Build());
    }
}
