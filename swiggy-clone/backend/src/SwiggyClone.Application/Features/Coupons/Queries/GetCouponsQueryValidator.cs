using FluentValidation;

namespace SwiggyClone.Application.Features.Coupons.Queries;

public sealed class GetCouponsQueryValidator : AbstractValidator<GetCouponsQuery>
{
    public GetCouponsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}
