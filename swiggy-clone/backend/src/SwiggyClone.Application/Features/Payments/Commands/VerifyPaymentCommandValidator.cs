using FluentValidation;

namespace SwiggyClone.Application.Features.Payments.Commands;

public sealed class VerifyPaymentCommandValidator : AbstractValidator<VerifyPaymentCommand>
{
    public VerifyPaymentCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.GatewayOrderId).NotEmpty().MaximumLength(255);
        RuleFor(x => x.GatewayPaymentId).NotEmpty().MaximumLength(255);
        RuleFor(x => x.GatewaySignature).NotEmpty().MaximumLength(512);
    }
}
