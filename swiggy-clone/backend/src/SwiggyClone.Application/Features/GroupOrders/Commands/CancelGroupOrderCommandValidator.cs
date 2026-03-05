using FluentValidation;

namespace SwiggyClone.Application.Features.GroupOrders.Commands;

public sealed class CancelGroupOrderCommandValidator : AbstractValidator<CancelGroupOrderCommand>
{
    public CancelGroupOrderCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.GroupOrderId).NotEmpty();
    }
}
