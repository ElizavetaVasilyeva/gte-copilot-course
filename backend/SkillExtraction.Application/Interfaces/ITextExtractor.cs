namespace SkillExtraction.Application.Interfaces;

/// <summary>
/// Interface for extracting text from document files (PDF, DOCX).
/// </summary>
public interface ITextExtractor
{
    /// <summary>
    /// Extracts text content from a document file.
    /// </summary>
    /// <param name="fileStream">The file stream to extract text from</param>
    /// <param name="fileName">The name of the file (used to determine file type)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The extracted text content</returns>
    Task<string> ExtractTextAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
}
