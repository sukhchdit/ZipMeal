using FluentValidation;

namespace SwiggyClone.Application.Features.Notifications.Commands;

public sealed class MarkAllNotificationsAsReadCommandValidator
    : AbstractValidator<MarkAllNotificationsAsReadCommand>
{
    public MarkAllNotificationsAsReadCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
