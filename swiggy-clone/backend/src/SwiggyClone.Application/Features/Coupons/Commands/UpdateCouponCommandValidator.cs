using FluentValidation;

namespace SwiggyClone.Application.Features.Coupons.Commands;

public sealed class UpdateCouponCommandValidator : AbstractValidator<UpdateCouponCommand>
{
    public UpdateCouponCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.MinOrderAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ValidFrom).LessThan(x => x.ValidUntil)
            .WithMessage("ValidFrom must be before ValidUntil.");
        RuleFor(x => x.MaxUsagesPerUser).GreaterThan(0);
    }
}
