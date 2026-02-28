using FluentValidation;

namespace SwiggyClone.Application.Features.Coupons.Commands;

public sealed class CreateCouponCommandValidator : AbstractValidator<CreateCouponCommand>
{
    public CreateCouponCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20)
            .Matches("^[A-Z0-9]+$").WithMessage("Code must be alphanumeric uppercase.");
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.DiscountType).InclusiveBetween(1, 2)
            .WithMessage("DiscountType must be 1 (Percentage) or 2 (FlatAmount).");
        RuleFor(x => x.DiscountValue).GreaterThan(0);
        RuleFor(x => x.DiscountValue).LessThanOrEqualTo(100)
            .When(x => x.DiscountType == 1)
            .WithMessage("Percentage discount must be 100 or less.");
        RuleFor(x => x.MinOrderAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ValidFrom).LessThan(x => x.ValidUntil)
            .WithMessage("ValidFrom must be before ValidUntil.");
        RuleFor(x => x.MaxUsagesPerUser).GreaterThan(0);
    }
}
