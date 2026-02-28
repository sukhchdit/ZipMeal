using FluentValidation;

namespace SwiggyClone.Application.Features.Orders.Commands;

public sealed class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.DeliveryAddressId).NotEmpty().WithMessage("Delivery address is required.");
        RuleFor(x => x.PaymentMethod)
            .InclusiveBetween(1, 6)
            .WithMessage("Payment method must be between 1 and 6.");
        RuleFor(x => x.SpecialInstructions)
            .MaximumLength(500).WithMessage("Special instructions must not exceed 500 characters.");
        RuleFor(x => x.CouponCode).MaximumLength(20)
            .When(x => !string.IsNullOrWhiteSpace(x.CouponCode));
    }
}
