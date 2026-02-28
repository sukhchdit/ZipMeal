using FluentValidation;

namespace SwiggyClone.Application.Features.Wallet.Commands.DebitWallet;

public sealed class DebitWalletCommandValidator : AbstractValidator<DebitWalletCommand>
{
    public DebitWalletCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.AmountPaise).GreaterThan(0);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
    }
}
