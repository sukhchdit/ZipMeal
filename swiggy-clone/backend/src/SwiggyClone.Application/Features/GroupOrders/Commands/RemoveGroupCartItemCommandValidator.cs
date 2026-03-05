using FluentValidation;

namespace SwiggyClone.Application.Features.GroupOrders.Commands;

public sealed class RemoveGroupCartItemCommandValidator : AbstractValidator<RemoveGroupCartItemCommand>
{
    public RemoveGroupCartItemCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.GroupOrderId).NotEmpty();
        RuleFor(x => x.CartItemId).NotEmpty();
    }
}
