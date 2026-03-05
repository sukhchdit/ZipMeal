using FluentValidation;

namespace SwiggyClone.Application.Features.GroupOrders.Commands;

public sealed class UpdateGroupCartItemCommandValidator : AbstractValidator<UpdateGroupCartItemCommand>
{
    public UpdateGroupCartItemCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.GroupOrderId).NotEmpty();
        RuleFor(x => x.CartItemId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0).LessThanOrEqualTo(20);
    }
}
