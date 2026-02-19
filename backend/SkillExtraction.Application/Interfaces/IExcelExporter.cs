using SkillExtraction.Domain.Entities;

namespace SkillExtraction.Application.Interfaces;

/// <summary>
/// Interface for exporting skills to Excel format.
/// </summary>
public interface IExcelExporter
{
    /// <summary>
    /// Exports skills to an Excel file in binary format.
    /// </summary>
    /// <param name="skills">The collection of skills to export</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A byte array containing the Excel file data</returns>
    Task<byte[]> ExportSkillsAsync(IEnumerable<ExtractedSkill> skills, CancellationToken cancellationToken = default);
}
