using FluentValidation;

namespace SwiggyClone.Application.Features.Notifications.Commands;

public sealed class RegisterDeviceCommandValidator : AbstractValidator<RegisterDeviceCommand>
{
    public RegisterDeviceCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.DeviceToken).NotEmpty().MaximumLength(512);
        RuleFor(x => x.Platform).InclusiveBetween(1, 3)
            .WithMessage("Platform must be 1 (Android), 2 (iOS), or 3 (Web).");
    }
}
