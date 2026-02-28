using FluentValidation;

namespace SwiggyClone.Application.Features.Subscriptions.Commands.TogglePlan;

public sealed class TogglePlanCommandValidator : AbstractValidator<TogglePlanCommand>
{
    public TogglePlanCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
