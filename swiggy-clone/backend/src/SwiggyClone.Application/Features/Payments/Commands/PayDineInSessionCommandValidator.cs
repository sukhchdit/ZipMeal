using FluentValidation;

namespace SwiggyClone.Application.Features.Payments.Commands;

public sealed class PayDineInSessionCommandValidator : AbstractValidator<PayDineInSessionCommand>
{
    public PayDineInSessionCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.SessionId).NotEmpty();
        RuleFor(x => x.PaymentMethod)
            .InclusiveBetween(1, 4)
            .WithMessage("Payment method must be an online method (1=UPI, 2=Card, 3=NetBanking, 4=Wallet).");
    }
}
