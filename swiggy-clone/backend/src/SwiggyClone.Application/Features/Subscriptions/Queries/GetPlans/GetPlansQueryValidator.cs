using FluentValidation;

namespace SwiggyClone.Application.Features.Subscriptions.Queries.GetPlans;

public sealed class GetPlansQueryValidator : AbstractValidator<GetPlansQuery>
{
    public GetPlansQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}
