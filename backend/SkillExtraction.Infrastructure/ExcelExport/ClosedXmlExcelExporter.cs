using ClosedXML.Excel;
using SkillExtraction.Application.Interfaces;
using SkillExtraction.Domain.Entities;

namespace SkillExtraction.Infrastructure.ExcelExport;

public sealed class ClosedXmlExcelExporter : IExcelExporter
{
    public Task<byte[]> ExportSkillsAsync(IEnumerable<ExtractedSkill> skills, CancellationToken cancellationToken)
    {
        var skillsList = skills.ToList();
        
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Extracted Skills");

        // Add header row with styling
        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;
        headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        worksheet.Cell(1, 1).Value = "Skill Name";
        worksheet.Cell(1, 2).Value = "Category";
        worksheet.Cell(1, 3).Value = "Confidence";
        worksheet.Cell(1, 4).Value = "Snippet";
        worksheet.Cell(1, 5).Value = "Notes";

        // Add data rows
        for (int i = 0; i < skillsList.Count; i++)
        {
            var skill = skillsList[i];
            var rowIndex = i + 2; // Start from row 2 (after header)

            worksheet.Cell(rowIndex, 1).Value = skill.Name;
            worksheet.Cell(rowIndex, 2).Value = skill.Category ?? "-";
            worksheet.Cell(rowIndex, 3).Value = skill.Confidence;
            worksheet.Cell(rowIndex, 4).Value = skill.Snippet;
            worksheet.Cell(rowIndex, 5).Value = skill.Notes ?? "";

            // Format confidence as percentage
            worksheet.Cell(rowIndex, 3).Style.NumberFormat.Format = "0%";
        }

        // Auto-fit columns
        worksheet.Columns().AdjustToContents();

        // Set column widths with max limits for better readability
        worksheet.Column(1).Width = Math.Min(worksheet.Column(1).Width, 30); // Skill Name
        worksheet.Column(2).Width = Math.Min(worksheet.Column(2).Width, 20); // Category
        worksheet.Column(3).Width = 12; // Confidence
        worksheet.Column(4).Width = Math.Min(worksheet.Column(4).Width, 50); // Snippet
        worksheet.Column(5).Width = Math.Min(worksheet.Column(5).Width, 40); // Notes

        // Enable text wrapping for snippet and notes columns
        worksheet.Column(4).Style.Alignment.WrapText = true;
        worksheet.Column(5).Style.Alignment.WrapText = true;

        // Add borders to all cells
        var dataRange = worksheet.Range(1, 1, skillsList.Count + 1, 5);
        dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        // Convert workbook to byte array
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return Task.FromResult(stream.ToArray());
    }
}
