namespace SkillExtraction.Application.Common;

/// <summary>
/// Result DTO for GetSkillDictionaryQuery containing all skills.
/// </summary>
public sealed record GetSkillDictionaryResultDto(
    IReadOnlyList<SkillDictionaryItemDto> Skills
);
