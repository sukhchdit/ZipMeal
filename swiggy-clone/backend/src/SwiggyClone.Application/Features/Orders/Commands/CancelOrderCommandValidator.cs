using FluentValidation;

namespace SwiggyClone.Application.Features.Orders.Commands;

public sealed class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.CancellationReason)
            .MaximumLength(500).WithMessage("Cancellation reason must not exceed 500 characters.");
    }
}
