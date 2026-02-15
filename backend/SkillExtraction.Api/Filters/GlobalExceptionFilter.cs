using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SkillExtraction.Application.Exceptions;

namespace SkillExtraction.Api.Filters;

/// <summary>
/// Global exception filter that provides consistent error handling across all API endpoints.
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

        switch (exception)
        {
            case ValidationException validationException:
                HandleValidationException(context, validationException);
                break;

            case UserAlreadyExistsException userExistsException:
                HandleUserAlreadyExistsException(context, userExistsException);
                break;

            case InvalidCredentialsException invalidCredentialsException:
                HandleInvalidCredentialsException(context, invalidCredentialsException);
                break;

            case UserNotFoundException userNotFoundException:
                HandleUserNotFoundException(context, userNotFoundException);
                break;

            case TextExtractionException textExtractionException:
                HandleTextExtractionException(context, textExtractionException);
                break;

            default:
                HandleUnknownException(context, exception);
                break;
        }

        context.ExceptionHandled = true;
    }

    private void HandleValidationException(ExceptionContext context, ValidationException exception)
    {
        _logger.LogWarning(exception, "Validation failed");

        var errors = exception.Errors.Select(e => new
        {
            property = e.PropertyName,
            message = e.ErrorMessage
        });

        context.Result = new BadRequestObjectResult(new
        {
            type = "ValidationError",
            title = "One or more validation errors occurred.",
            errors
        });
    }

    private void HandleUserAlreadyExistsException(ExceptionContext context, UserAlreadyExistsException exception)
    {
        _logger.LogWarning(exception, "User already exists: {Username}", exception.Username);

        context.Result = new ConflictObjectResult(new
        {
            type = "UserAlreadyExists",
            title = "Username conflict",
            message = exception.Message,
            username = exception.Username
        });
    }

    private void HandleInvalidCredentialsException(ExceptionContext context, InvalidCredentialsException exception)
    {
        _logger.LogWarning(exception, "Invalid credentials provided");

        context.Result = new UnauthorizedObjectResult(new
        {
            type = "InvalidCredentials",
            title = "Authentication failed",
            message = exception.Message
        });
    }

    private void HandleUserNotFoundException(ExceptionContext context, UserNotFoundException exception)
    {
        _logger.LogWarning(exception, "User not found: {Username}", exception.Username);

        context.Result = new NotFoundObjectResult(new
        {
            type = "UserNotFound",
            title = "User not found",
            message = exception.Message,
            username = exception.Username
        });
    }

    private void HandleTextExtractionException(ExceptionContext context, TextExtractionException exception)
    {
        _logger.LogError(exception, "Text extraction failed for file: {FileName}", exception.FileName);

        context.Result = new BadRequestObjectResult(new
        {
            type = "TextExtractionError",
            title = "Failed to extract text from document",
            message = exception.Message,
            fileName = exception.FileName
        });
    }

    private void HandleUnknownException(ExceptionContext context, Exception exception)
    {
        _logger.LogError(exception, "Unhandled exception of type {ExceptionType}", exception.GetType().Name);

        context.Result = new ObjectResult(new
        {
            type = "InternalServerError",
            title = "An error occurred while processing your request.",
            message = "An unexpected error occurred. Please try again later."
        })
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }
}
