using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillExtraction.Application.Interfaces;
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
        // Persistence
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();

        // Security
        services.AddSingleton<IPasswordHasher, PasswordHasherAdapter>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        // Text Extraction
        services.AddSingleton<ITextExtractor, CompositeTextExtractor>();

        // Skill Matching
        services.AddSingleton<InMemorySkillDictionary>();
        services.AddSingleton<ISkillExtractor, DictionaryBasedSkillExtractor>();

        // Excel Export
        services.AddSingleton<IExcelExporter, ClosedXmlExcelExporter>();

        return services;
    }
}
