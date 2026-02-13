using MediatR;

namespace SkillExtraction.Application.Commands.Skills.ExtractSkills;

/// <summary>
/// Command to extract skills from uploaded CV and optional IFU documents.
/// </summary>
public record ExtractSkillsCommand : IRequest<ExtractSkillsDto>
{
    public Stream CvFileStream { get; init; } = null!;
    public string CvFileName { get; init; } = null!;
    public Stream? IfuFileStream { get; init; }
    public string? IfuFileName { get; init; }
}
