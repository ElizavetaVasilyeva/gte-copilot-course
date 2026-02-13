using FluentValidation;

namespace SkillExtraction.Application.Commands.Skills.ExtractSkills;

/// <summary>
/// Validator for ExtractSkillsCommand.
/// </summary>
public class ExtractSkillsCommandValidator : AbstractValidator<ExtractSkillsCommand>
{
    private static readonly string[] AllowedExtensions = { ".pdf", ".docx" };

    public ExtractSkillsCommandValidator()
    {
        RuleFor(x => x.CvFileStream)
            .NotNull().WithMessage("CV file stream is required.");

        RuleFor(x => x.CvFileName)
            .NotEmpty().WithMessage("CV file name is required.")
            .Must(HaveValidExtension).WithMessage("CV file must be a PDF or DOCX.");

        RuleFor(x => x.IfuFileName)
            .Must(HaveValidExtension).WithMessage("IFU file must be a PDF or DOCX.")
            .When(x => !string.IsNullOrWhiteSpace(x.IfuFileName));
    }

    private bool HaveValidExtension(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return false;

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }
}
