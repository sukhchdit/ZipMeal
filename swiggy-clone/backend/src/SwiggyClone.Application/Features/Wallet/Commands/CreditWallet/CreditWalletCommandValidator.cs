using FluentValidation;

namespace SwiggyClone.Application.Features.Wallet.Commands.CreditWallet;

public sealed class CreditWalletCommandValidator : AbstractValidator<CreditWalletCommand>
{
    public CreditWalletCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.AmountPaise).GreaterThan(0);
        RuleFor(x => x.Source).InclusiveBetween((short)0, (short)6);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
    }
}
