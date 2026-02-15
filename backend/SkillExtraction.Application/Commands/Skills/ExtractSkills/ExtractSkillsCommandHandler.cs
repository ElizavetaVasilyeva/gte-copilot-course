using MediatR;
using Microsoft.Extensions.Logging;
using SkillExtraction.Application.Interfaces;

namespace SkillExtraction.Application.Commands.Skills.ExtractSkills;

/// <summary>
/// Handler for ExtractSkillsCommand.
/// Orchestrates text extraction from documents and skill matching.
/// </summary>
public class ExtractSkillsCommandHandler : IRequestHandler<ExtractSkillsCommand, ExtractSkillsDto>
{
    private readonly ITextExtractor _textExtractor;
    private readonly ISkillExtractor _skillExtractor;
    private readonly ILogger<ExtractSkillsCommandHandler> _logger;

    public ExtractSkillsCommandHandler(
        ITextExtractor textExtractor,
        ISkillExtractor skillExtractor,
        ILogger<ExtractSkillsCommandHandler> logger)
    {
        _textExtractor = textExtractor ?? throw new ArgumentNullException(nameof(textExtractor));
        _skillExtractor = skillExtractor ?? throw new ArgumentNullException(nameof(skillExtractor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the skill extraction command by extracting text from documents and matching skills.
    /// </summary>
    /// <param name="request">The command containing CV and optional IFU file streams</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Extracted skills with metadata</returns>
    public async Task<ExtractSkillsDto> Handle(ExtractSkillsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Extracting skills from uploaded documents");

        // Extract text from CV file
        var cvText = await _textExtractor.ExtractTextAsync(
            request.CvFileStream,
            request.CvFileName,
            cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Extracted {Length} characters from CV", cvText.Length);

        // Extract text from optional IFU file
        string ifuText = string.Empty;
        if (request.IfuFileStream != null && !string.IsNullOrWhiteSpace(request.IfuFileName))
        {
            ifuText = await _textExtractor.ExtractTextAsync(
                request.IfuFileStream,
                request.IfuFileName,
                cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Extracted {Length} characters from IFU", ifuText.Length);
        }

        // Combine texts
        var combinedText = $"{cvText}\n\n{ifuText}".Trim();

        // Extract skills from combined text
        var extractedSkills = await _skillExtractor.ExtractSkillsAsync(combinedText, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Extracted {Count} skills from documents", extractedSkills.Count());

        // Map to DTOs
        var skillDtos = extractedSkills.Select(skill => new ExtractedSkillDto
        {
            Name = skill.Name,
            Category = skill.Category,
            Confidence = skill.Confidence,
            Snippet = skill.Snippet,
            Notes = skill.Notes
        }).ToList();

        return new ExtractSkillsDto
        {
            Skills = skillDtos,
            RawTextLength = combinedText.Length
        };
    }
}
