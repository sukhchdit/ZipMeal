using FluentValidation;

namespace SwiggyClone.Application.Features.Cart.Commands;

public sealed class UpdateCartItemQuantityCommandValidator : AbstractValidator<UpdateCartItemQuantityCommand>
{
    public UpdateCartItemQuantityCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CartItemId).NotEmpty().WithMessage("Cart item ID is required.");
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0).WithMessage("Quantity must be 0 or greater.");
    }
}
