namespace SkillExtraction.Application.Common;

/// <summary>
/// DTO representing a single skill definition in the dictionary.
/// </summary>
public sealed record SkillDictionaryItemDto(
    string Name,
    string Category,
    IReadOnlyList<string> Aliases
);
