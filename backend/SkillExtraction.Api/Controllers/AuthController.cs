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

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
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
        var result = await _mediator.Send(command);
        return Ok(result);
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
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
