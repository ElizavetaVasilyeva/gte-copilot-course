using SkillExtraction.Domain.Entities;

namespace SkillExtraction.Application.Interfaces;

/// <summary>
/// Interface for extracting skills from text.
/// </summary>
public interface ISkillExtractor
{
    Task<IEnumerable<ExtractedSkill>> ExtractSkillsAsync(string text, CancellationToken cancellationToken = default);
}
