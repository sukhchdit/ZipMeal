using FluentValidation;

namespace SwiggyClone.Application.Features.Coupons.Commands;

public sealed class ToggleCouponCommandValidator : AbstractValidator<ToggleCouponCommand>
{
    public ToggleCouponCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
