using FluentValidation;

namespace DependencyFinder.Tool.Modules.Validation;

public class OptionsValidator : AbstractValidator<Options>
{
    public OptionsValidator()
    {
        RuleFor(x => x.InputPath)
            .NotEmpty().WithMessage("InputPath is required.")
            .Must(IsValidPath).WithMessage("InputPath must be a valid SQL file and not empty.");
        RuleFor(x => x.OutputPath)
            .NotNull().WithMessage("OutputPath is required.")
            .Must(IsValidDirectory).WithMessage("OutputPath must be a valid directory.");
        RuleFor(x => x.Verbose)
            .NotNull().WithMessage("Verbose is required.");
    }
}