using Microsoft.AspNetCore.Identity;
using SkillExtraction.Application.Interfaces;

namespace SkillExtraction.Infrastructure.Security;

/// <summary>
/// Password hasher adapter using ASP.NET Core Identity's PasswordHasher.
/// </summary>
public class PasswordHasherAdapter : IPasswordHasher
{
    private readonly PasswordHasher<object> _passwordHasher = new();

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(null!, password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var result = _passwordHasher.VerifyHashedPassword(null!, passwordHash, password);
        return result == PasswordVerificationResult.Success || 
               result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}
