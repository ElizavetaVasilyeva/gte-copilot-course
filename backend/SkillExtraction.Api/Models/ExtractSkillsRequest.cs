using Microsoft.AspNetCore.Mvc;

namespace SkillExtraction.Api.Models;

/// <summary>
/// Request model for skill extraction with file uploads.
/// </summary>
public class ExtractSkillsRequest
{
    /// <summary>
    /// Required CV document (PDF or DOCX).
    /// </summary>
    [FromForm(Name = "cvFile")]
    public IFormFile CvFile { get; set; } = null!;

    /// <summary>
    /// Optional IFU document (PDF or DOCX).
    /// </summary>
    [FromForm(Name = "ifuFile")]
    public IFormFile? IfuFile { get; set; }
}
