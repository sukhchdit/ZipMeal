using FluentValidation;

namespace SwiggyClone.Application.Features.Coupons.Queries;

public sealed class ValidateCouponQueryValidator : AbstractValidator<ValidateCouponQuery>
{
    public ValidateCouponQueryValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.OrderSubtotal).GreaterThan(0);
        RuleFor(x => x.RestaurantId).NotEmpty();
    }
}
