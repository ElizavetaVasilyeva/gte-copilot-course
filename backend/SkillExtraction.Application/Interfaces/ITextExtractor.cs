namespace SkillExtraction.Application.Interfaces;

/// <summary>
/// Interface for extracting text from document files (PDF, DOCX).
/// </summary>
public interface ITextExtractor
{
    Task<string> ExtractTextAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
}
