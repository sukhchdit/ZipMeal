using FluentValidation;

namespace SwiggyClone.Application.Features.Deliveries.Commands;

public sealed class ToggleOnlineStatusCommandValidator : AbstractValidator<ToggleOnlineStatusCommand>
{
    public ToggleOnlineStatusCommandValidator()
    {
        RuleFor(x => x.PartnerId).NotEmpty();
        When(x => x.IsOnline, () =>
        {
            RuleFor(x => x.Latitude).NotNull()
                .WithMessage("Latitude is required when going online.");
            RuleFor(x => x.Longitude).NotNull()
                .WithMessage("Longitude is required when going online.");
        });
    }
}
