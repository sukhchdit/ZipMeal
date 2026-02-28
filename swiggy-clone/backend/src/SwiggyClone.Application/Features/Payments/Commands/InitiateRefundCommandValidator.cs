using FluentValidation;

namespace SwiggyClone.Application.Features.Payments.Commands;

public sealed class InitiateRefundCommandValidator : AbstractValidator<InitiateRefundCommand>
{
    public InitiateRefundCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Reason).MaximumLength(500);
    }
}
