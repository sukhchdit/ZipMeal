using FluentValidation;

namespace SwiggyClone.Application.Features.Subscriptions.Commands.CancelSubscription;

public sealed class CancelSubscriptionCommandValidator : AbstractValidator<CancelSubscriptionCommand>
{
    public CancelSubscriptionCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
