namespace SkillExtraction.Application.Exceptions;

/// <summary>
/// Exception thrown when text extraction from a document fails.
/// </summary>
public class TextExtractionException : Exception
{
    public string? FileName { get; }

    public TextExtractionException(string message)
        : base(message)
    {
    }

    public TextExtractionException(string message, string fileName)
        : base(message)
    {
        FileName = fileName;
    }

    public TextExtractionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public TextExtractionException(string message, string fileName, Exception innerException)
        : base(message, innerException)
    {
        FileName = fileName;
    }
}
