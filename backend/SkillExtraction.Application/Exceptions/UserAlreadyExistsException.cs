namespace SkillExtraction.Application.Exceptions;

/// <summary>
/// Exception thrown when attempting to create a user with a username that already exists.
/// </summary>
public class UserAlreadyExistsException : Exception
{
    public string Username { get; }

    public UserAlreadyExistsException(string username)
        : base($"Username '{username}' is already taken.")
    {
        Username = username;
    }

    public UserAlreadyExistsException(string username, Exception innerException)
        : base($"Username '{username}' is already taken.", innerException)
    {
        Username = username;
    }
}
