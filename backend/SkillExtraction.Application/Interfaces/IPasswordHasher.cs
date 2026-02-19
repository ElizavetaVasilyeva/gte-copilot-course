namespace SkillExtraction.Application.Interfaces;

/// <summary>
/// Interface for password hashing operations.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a plaintext password using PBKDF2.
    /// </summary>
    /// <param name="password">The plaintext password to hash</param>
    /// <returns>The hashed password string</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a plaintext password against a stored hash.
    /// </summary>
    /// <param name="password">The plaintext password to verify</param>
    /// <param name="passwordHash">The stored password hash</param>
    /// <returns>True if the password matches the hash, otherwise false</returns>
    bool VerifyPassword(string password, string passwordHash);
}
