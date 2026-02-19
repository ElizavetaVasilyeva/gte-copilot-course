using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillExtraction.Application.Interfaces;
using SkillExtraction.Infrastructure.Configuration;
using SkillExtraction.Infrastructure.ExcelExport;
using SkillExtraction.Infrastructure.Persistence;
using SkillExtraction.Infrastructure.Security;
using SkillExtraction.Infrastructure.SkillMatching;
using SkillExtraction.Infrastructure.TextExtraction;

namespace SkillExtraction.Infrastructure;

/// <summary>
/// Extension methods for configuring Infrastructure layer services.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure strongly-typed settings with manual binding
        var jwtSection = configuration.GetSection("JwtSettings");
        services.Configure<JwtSettings>(options =>
        {
            options.Secret = jwtSection["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
            options.Issuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
            options.Audience = jwtSection["Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");
            options.ExpirationInMinutes = int.Parse(jwtSection["ExpirationInMinutes"] ?? "60");
        });

        var dbSection = configuration.GetSection("DatabaseSettings");
        services.Configure<DatabaseSettings>(options =>
        {
            options.Provider = dbSection["Provider"] ?? "InMemory";
            options.ConnectionString = dbSection["ConnectionString"] ?? string.Empty;
        });

        // Persistence - Choose between InMemory or SQLite based on configuration
        var databaseProvider = configuration["DatabaseSettings:Provider"] ?? "InMemory";
        
        if (databaseProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
        {
            var connectionString = configuration["DatabaseSettings:ConnectionString"];
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));
            
            services.AddScoped<IUserRepository, SqliteUserRepository>();
        }
        else
        {
            // Default to in-memory for testing or when no database is configured
            services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        }

        // Security
        services.AddSingleton<IPasswordHasher, PasswordHasherAdapter>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        // Text Extraction
        services.AddSingleton<ITextExtractor, CompositeTextExtractor>();

        // Skill Matching
        var skillDictionary = new InMemorySkillDictionary();
        services.AddSingleton<InMemorySkillDictionary>(skillDictionary);
        services.AddSingleton<ISkillDictionary>(skillDictionary);
        services.AddSingleton<ISkillExtractor, DictionaryBasedSkillExtractor>();

        // Excel Export
        services.AddSingleton<IExcelExporter, ClosedXmlExcelExporter>();

        return services;
    }
}
