using FluentValidation;

namespace DependencyFinder.Tool.Modules.Validation;

public class OptionsValidator : AbstractValidator<Options>
{
    public OptionsValidator()
    {
        RuleFor(x => x.InputPath)
            .NotEmpty().WithMessage("InputPath is required.")
            .Must(IsValidFile).WithMessage("InputPath must be a valid SQL file and not empty.");
    }
}