using FluentValidation;

namespace SwiggyClone.Application.Features.Subscriptions.Commands.CreatePlan;

public sealed class CreatePlanCommandValidator : AbstractValidator<CreatePlanCommand>
{
    public CreatePlanCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PricePaise).GreaterThan(0);
        RuleFor(x => x.DurationDays).InclusiveBetween(1, 365);
        RuleFor(x => x.ExtraDiscountPercent).InclusiveBetween(0, 100);
    }
}
