using FluentValidation;

namespace SwiggyClone.Application.Features.DineIn.Commands;

public sealed class StartSessionCommandValidator : AbstractValidator<StartSessionCommand>
{
    public StartSessionCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.QrCodeData).NotEmpty().MaximumLength(500)
            .WithMessage("QR code data is required.");
        RuleFor(x => x.GuestCount).InclusiveBetween(1, 20)
            .WithMessage("Guest count must be between 1 and 20.");
    }
}
