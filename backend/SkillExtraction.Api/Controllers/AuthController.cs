using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SkillExtraction.Application.Commands.Auth.SignIn;
using SkillExtraction.Application.Commands.Auth.SignUp;

namespace SkillExtraction.Api.Controllers;

/// <summary>
/// Authentication controller for user signup and signin.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user.
    /// </summary>
    /// <param name="command">Sign up credentials</param>
    /// <returns>JWT token</returns>
    [HttpPost("signup")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SignUp([FromBody] SignUpCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for signup request");
            return BadRequest(new
            {
                errors = ex.Errors.Select(e => new
                {
                    property = e.PropertyName,
                    message = e.ErrorMessage
                })
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already taken") || ex.Message.Contains("already exists"))
        {
            _logger.LogWarning(ex, "Username conflict during signup");
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during signup");
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "An error occurred while processing your request."
            });
        }
    }

    /// <summary>
    /// Sign in an existing user.
    /// </summary>
    /// <param name="command">Sign in credentials</param>
    /// <returns>JWT token</returns>
    [HttpPost("signin")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SignIn([FromBody] SignInCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for signin request");
            return BadRequest(new
            {
                errors = ex.Errors.Select(e => new
                {
                    property = e.PropertyName,
                    message = e.ErrorMessage
                })
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Invalid credentials during signin");
            return Unauthorized(new { message = "Invalid username or password." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during signin");
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "An error occurred while processing your request."
            });
        }
    }
}
