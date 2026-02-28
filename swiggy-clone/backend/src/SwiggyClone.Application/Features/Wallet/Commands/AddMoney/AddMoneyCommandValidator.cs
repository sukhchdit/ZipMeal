using FluentValidation;

namespace SwiggyClone.Application.Features.Wallet.Commands.AddMoney;

public sealed class AddMoneyCommandValidator : AbstractValidator<AddMoneyCommand>
{
    public AddMoneyCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.AmountPaise).GreaterThan(0)
            .LessThanOrEqualTo(1_000_000)
            .WithMessage("Amount must be between 1 paise and ₹10,000.");
    }
}
