namespace SkillExtraction.Application.Commands.Skills.ExtractSkills;

/// <summary>
/// DTO containing extracted skills and metadata.
/// </summary>
public record ExtractSkillsDto
{
    public IEnumerable<ExtractedSkillDto> Skills { get; init; } = Array.Empty<ExtractedSkillDto>();
    public int RawTextLength { get; init; }
}

/// <summary>
/// DTO representing a single extracted skill.
/// </summary>
public record ExtractedSkillDto
{
    public string Name { get; init; } = null!;
    public string? Category { get; init; }
    public double Confidence { get; init; }
    public string Snippet { get; init; } = null!;
    public string? Notes { get; init; }
}
