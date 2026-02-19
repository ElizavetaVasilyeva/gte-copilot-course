using SkillExtraction.Application.Commands.Skills.ExportSkills;

namespace SkillExtraction.Api.Models;

/// <summary>
/// Request model for exporting skills to Excel.
/// </summary>
public sealed class ExportSkillsRequest
{
    /// <summary>
    /// List of skills to export.
    /// </summary>
    public required IReadOnlyList<ExportSkillDto> Skills { get; init; }
}
