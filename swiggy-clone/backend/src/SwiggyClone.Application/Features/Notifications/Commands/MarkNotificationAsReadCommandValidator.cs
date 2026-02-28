using FluentValidation;

namespace SwiggyClone.Application.Features.Notifications.Commands;

public sealed class MarkNotificationAsReadCommandValidator
    : AbstractValidator<MarkNotificationAsReadCommand>
{
    public MarkNotificationAsReadCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.NotificationId).NotEmpty();
    }
}
