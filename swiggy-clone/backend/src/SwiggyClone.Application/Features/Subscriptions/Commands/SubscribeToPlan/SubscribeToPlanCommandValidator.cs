using FluentValidation;

namespace SwiggyClone.Application.Features.Subscriptions.Commands.SubscribeToPlan;

public sealed class SubscribeToPlanCommandValidator : AbstractValidator<SubscribeToPlanCommand>
{
    public SubscribeToPlanCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.PlanId).NotEmpty();
    }
}
