using FluentValidation;

namespace SkillExtraction.Application.Commands.Auth.SignIn;

/// <summary>
/// Validator for SignInCommand.
/// </summary>
public class SignInCommandValidator : AbstractValidator<SignInCommand>
{
    public SignInCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
