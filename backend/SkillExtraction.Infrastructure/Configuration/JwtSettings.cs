using System.ComponentModel.DataAnnotations;

namespace SkillExtraction.Infrastructure.Configuration;

/// <summary>
/// Strongly-typed configuration for JWT authentication settings.
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Secret key for JWT token signing. Must be at least 32 characters.
    /// </summary>
    [Required(ErrorMessage = "JWT Secret is required")]
    [MinLength(32, ErrorMessage = "JWT Secret must be at least 32 characters")]
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// JWT token issuer identifier.
    /// </summary>
    [Required(ErrorMessage = "JWT Issuer is required")]
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// JWT token audience identifier.
    /// </summary>
    [Required(ErrorMessage = "JWT Audience is required")]
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration time in minutes. Must be between 1 and 1440 (24 hours).
    /// </summary>
    [Range(1, 1440, ErrorMessage = "Expiration must be between 1 and 1440 minutes")]
    public int ExpirationInMinutes { get; set; } = 60;
}
