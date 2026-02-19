using FluentValidation;

namespace SkillExtraction.Application.Commands.Skills.ExportSkills;

public sealed class ExportSkillsCommandValidator : AbstractValidator<ExportSkillsCommand>
{
    public ExportSkillsCommandValidator()
    {
        RuleFor(x => x.Skills)
            .NotNull()
            .WithMessage("Skills list cannot be null.");

        RuleFor(x => x.Skills)
            .NotEmpty()
            .WithMessage("Skills list cannot be empty. At least one skill is required to export.");

        RuleForEach(x => x.Skills)
            .ChildRules(skill =>
            {
                skill.RuleFor(s => s.Name)
                    .NotEmpty()
                    .WithMessage("Skill name cannot be empty.");

                skill.RuleFor(s => s.Confidence)
                    .InclusiveBetween(0, 1)
                    .WithMessage("Confidence must be between 0 and 1.");

                skill.RuleFor(s => s.Snippet)
                    .NotEmpty()
                    .WithMessage("Skill snippet cannot be empty.");
            });
    }
}
