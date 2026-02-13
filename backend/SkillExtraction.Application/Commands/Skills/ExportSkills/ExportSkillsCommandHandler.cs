using MediatR;
using SkillExtraction.Application.Interfaces;
using SkillExtraction.Domain.Entities;

namespace SkillExtraction.Application.Commands.Skills.ExportSkills;

public sealed class ExportSkillsCommandHandler : IRequestHandler<ExportSkillsCommand, ExportSkillsResultDto>
{
    private readonly IExcelExporter _excelExporter;

    public ExportSkillsCommandHandler(IExcelExporter excelExporter)
    {
        _excelExporter = excelExporter;
    }

    public async Task<ExportSkillsResultDto> Handle(ExportSkillsCommand request, CancellationToken cancellationToken)
    {
        // Map DTOs to domain entities
        var skills = request.Skills.Select(dto => new ExtractedSkill(
            name: dto.Name,
            category: dto.Category,
            confidence: dto.Confidence,
            snippet: dto.Snippet,
            notes: dto.Notes
        )).ToList();

        // Generate Excel file
        var fileBytes = await _excelExporter.ExportSkillsAsync(skills, cancellationToken);

        // Return result with file metadata
        var fileName = $"ExtractedSkills_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        return new ExportSkillsResultDto(
            FileContents: fileBytes,
            FileName: fileName,
            ContentType: contentType
        );
    }
}
