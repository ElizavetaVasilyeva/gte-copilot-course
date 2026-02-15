namespace SkillExtraction.Application.Exceptions;

/// <summary>
/// Exception thrown when a requested user cannot be found.
/// </summary>
public class UserNotFoundException : Exception
{
    public string Username { get; }

    public UserNotFoundException(string username)
        : base($"User '{username}' not found.")
    {
        Username = username;
    }

    public UserNotFoundException(string username, Exception innerException)
        : base($"User '{username}' not found.", innerException)
    {
        Username = username;
    }
}
