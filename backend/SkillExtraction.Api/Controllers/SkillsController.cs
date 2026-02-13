using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SkillExtraction.Api.Models;
using SkillExtraction.Application.Commands.Skills.ExtractSkills;
using SkillExtraction.Application.Commands.Skills.ExportSkills;

namespace SkillExtraction.Api.Controllers;

/// <summary>
/// Controller for skill extraction and export operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SkillsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SkillsController> _logger;

    public SkillsController(IMediator mediator, ILogger<SkillsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Extracts skills from uploaded CV and optional IFU documents.
    /// </summary>
    /// <param name="request">Request containing CV and optional IFU files</param>
    /// <returns>Extracted skills with metadata</returns>
    [HttpPost("extract")]
    [SwaggerOperation(Summary = "Extract skills from documents", Description = "Extracts skills from uploaded CV and optional IFU documents (PDF or DOCX format)")]
    [SwaggerResponse(StatusCodes.Status200OK, "Skills extracted successfully", typeof(ExtractSkillsDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ExtractSkills([FromForm] ExtractSkillsRequest request)
    {
        try
        {
            await using var cvStream = request.CvFile.OpenReadStream();
            Stream? ifuStream = null;
            
            if (request.IfuFile != null)
            {
                ifuStream = request.IfuFile.OpenReadStream();
            }

            var command = new ExtractSkillsCommand
            {
                CvFileStream = cvStream,
                CvFileName = request.CvFile.FileName,
                IfuFileStream = ifuStream,
                IfuFileName = request.IfuFile?.FileName
            };

            var result = await _mediator.Send(command);

            if (ifuStream != null)
            {
                await ifuStream.DisposeAsync();
            }

            return Ok(result);
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for extract skills request");
            return BadRequest(new { errors = ex.Errors.Select(e => e.ErrorMessage) });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting skills from documents");
            return BadRequest(new { error = "Failed to extract skills from documents." });
        }
    }

    /// <summary>
    /// Exports skills to Excel file.
    /// </summary>
    /// <param name="request">Request containing the list of skills to export</param>
    /// <returns>Excel file download</returns>
    [HttpPost("export")]
    [SwaggerOperation(Summary = "Export skills to Excel", Description = "Exports the provided list of skills to an Excel file (.xlsx)")]
    [SwaggerResponse(StatusCodes.Status200OK, "Excel file generated successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
    [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    public async Task<IActionResult> ExportSkills([FromBody] ExportSkillsRequest request)
    {
        try
        {
            var command = new ExportSkillsCommand(request.Skills);
            var result = await _mediator.Send(command);

            return File(result.FileContents, result.ContentType, result.FileName);
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for export skills request");
            return BadRequest(new { errors = ex.Errors.Select(e => e.ErrorMessage) });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting skills to Excel");
            return BadRequest(new { error = "Failed to export skills to Excel." });
        }
    }
}
