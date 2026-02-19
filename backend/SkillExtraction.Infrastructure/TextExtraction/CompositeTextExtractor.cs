using SkillExtraction.Application.Interfaces;

namespace SkillExtraction.Infrastructure.TextExtraction;

/// <summary>
/// Composite text extractor that delegates to specific extractors based on file extension.
/// </summary>
public class CompositeTextExtractor : ITextExtractor
{
    public async Task<string> ExtractTextAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        return extension switch
        {
            ".pdf" => await ExtractPdfTextAsync(fileStream, cancellationToken),
            ".docx" => await ExtractDocxTextAsync(fileStream, cancellationToken),
            _ => throw new NotSupportedException($"File type {extension} is not supported.")
        };
    }

    private static async Task<string> ExtractPdfTextAsync(Stream stream, CancellationToken cancellationToken)
    {
        try
        {
            using var document = UglyToad.PdfPig.PdfDocument.Open(stream);
            var textBuilder = new System.Text.StringBuilder();

            foreach (var page in document.GetPages())
            {
                textBuilder.AppendLine(page.Text);
            }

            return await Task.FromResult(textBuilder.ToString());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to extract text from PDF.", ex);
        }
    }

    private static async Task<string> ExtractDocxTextAsync(Stream stream, CancellationToken cancellationToken)
    {
        try
        {
            using var doc = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(stream, false);
            var body = doc.MainDocumentPart?.Document.Body;

            if (body == null)
                return string.Empty;

            var text = body.InnerText;
            return await Task.FromResult(text);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to extract text from DOCX.", ex);
        }
    }
}
