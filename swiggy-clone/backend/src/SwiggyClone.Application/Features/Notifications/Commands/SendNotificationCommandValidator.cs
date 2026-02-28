using FluentValidation;

namespace SwiggyClone.Application.Features.Notifications.Commands;

public sealed class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
{
    public SendNotificationCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Body).NotEmpty();
        RuleFor(x => x.Type).InclusiveBetween(1, 4)
            .WithMessage("Type must be between 1 (OrderUpdate) and 4 (System).");
    }
}
