using SkillExtraction.Domain.Entities;

namespace SkillExtraction.Application.Interfaces;

/// <summary>
/// Interface for accessing the skill dictionary.
/// </summary>
public interface ISkillDictionary
{
    /// <summary>
    /// Gets all skill definitions from the dictionary.
    /// </summary>
    IReadOnlyList<SkillDefinition> GetAllSkills();
}
