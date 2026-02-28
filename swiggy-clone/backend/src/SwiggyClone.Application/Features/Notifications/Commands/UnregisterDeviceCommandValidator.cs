using FluentValidation;

namespace SwiggyClone.Application.Features.Notifications.Commands;

public sealed class UnregisterDeviceCommandValidator : AbstractValidator<UnregisterDeviceCommand>
{
    public UnregisterDeviceCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.DeviceToken).NotEmpty().MaximumLength(512);
    }
}
