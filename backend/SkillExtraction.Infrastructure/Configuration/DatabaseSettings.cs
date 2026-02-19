using System.ComponentModel.DataAnnotations;

namespace SkillExtraction.Infrastructure.Configuration;

/// <summary>
/// Strongly-typed configuration for database settings.
/// </summary>
public class DatabaseSettings
{
    /// <summary>
    /// Database provider type (e.g., "Sqlite", "InMemory").
    /// </summary>
    [Required(ErrorMessage = "Database provider is required")]
    public string Provider { get; set; } = "InMemory";

    /// <summary>
    /// Database connection string. Required for Sqlite provider.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}
