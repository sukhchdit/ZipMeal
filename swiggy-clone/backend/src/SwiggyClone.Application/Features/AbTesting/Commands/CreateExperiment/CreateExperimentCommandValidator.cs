using FluentValidation;

namespace SwiggyClone.Application.Features.AbTesting.Commands.CreateExperiment;

public sealed class CreateExperimentCommandValidator : AbstractValidator<CreateExperimentCommand>
{
    public CreateExperimentCommandValidator()
    {
        RuleFor(x => x.CreatedByUserId).NotEmpty();
        RuleFor(x => x.Key).NotEmpty().MaximumLength(100)
            .Matches("^[a-z0-9_]+$").WithMessage("Key must be lowercase alphanumeric with underscores.");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.TargetAudience).MaximumLength(500);
        RuleFor(x => x.GoalDescription).MaximumLength(1000);
        RuleFor(x => x.Variants).NotEmpty()
            .WithMessage("At least one variant is required.");
        RuleFor(x => x.Variants.Sum(v => v.AllocationPercent))
            .Equal(100)
            .WithMessage("Variant allocations must sum to 100.");
        RuleFor(x => x.Variants.Count(v => v.IsControl))
            .Equal(1)
            .WithMessage("Exactly one variant must be marked as control.");
        RuleForEach(x => x.Variants).ChildRules(v =>
        {
            v.RuleFor(vi => vi.Key).NotEmpty().MaximumLength(100);
            v.RuleFor(vi => vi.Name).NotEmpty().MaximumLength(200);
            v.RuleFor(vi => vi.AllocationPercent).InclusiveBetween(0, 100);
        });
    }
}
