using FluentValidation;

namespace SwiggyClone.Application.Features.Tips.Commands;

public sealed class SubmitTipCommandValidator : AbstractValidator<SubmitTipCommand>
{
    public SubmitTipCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.AmountPaise).GreaterThan(0).LessThanOrEqualTo(50000);
    }
}
