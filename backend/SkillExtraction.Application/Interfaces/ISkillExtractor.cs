using SkillExtraction.Domain.Entities;

namespace SkillExtraction.Application.Interfaces;

/// <summary>
/// Interface for extracting skills from text.
/// </summary>
public interface ISkillExtractor
{
    /// <summary>
    /// Extracts skills from the provided text using dictionary matching.
    /// </summary>
    /// <param name="text">The text to extract skills from</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of extracted skills with confidence scores</returns>
    Task<IEnumerable<ExtractedSkill>> ExtractSkillsAsync(string text, CancellationToken cancellationToken = default);
}
