using FluentValidation;

namespace SwiggyClone.Application.Features.Subscriptions.Commands.UpdatePlan;

public sealed class UpdatePlanCommandValidator : AbstractValidator<UpdatePlanCommand>
{
    public UpdatePlanCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PricePaise).GreaterThan(0);
        RuleFor(x => x.DurationDays).InclusiveBetween(1, 365);
        RuleFor(x => x.ExtraDiscountPercent).InclusiveBetween(0, 100);
    }
}
