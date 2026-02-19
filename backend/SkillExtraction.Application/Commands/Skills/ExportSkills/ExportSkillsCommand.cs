using MediatR;

namespace SkillExtraction.Application.Commands.Skills.ExportSkills;

public sealed record ExportSkillsCommand(
    IReadOnlyList<ExportSkillDto> Skills
) : IRequest<ExportSkillsResultDto>;

public sealed record ExportSkillDto(
    string Name,
    string? Category,
    double Confidence,
    string Snippet,
    string? Notes
);

public sealed record ExportSkillsResultDto(
    byte[] FileContents,
    string FileName,
    string ContentType
);
