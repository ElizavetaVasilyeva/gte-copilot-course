using SkillExtraction.Domain.Entities;

namespace SkillExtraction.Application.Interfaces;

/// <summary>
/// Interface for exporting skills to Excel format.
/// </summary>
public interface IExcelExporter
{
    Task<byte[]> ExportSkillsAsync(IEnumerable<ExtractedSkill> skills, CancellationToken cancellationToken = default);
}
