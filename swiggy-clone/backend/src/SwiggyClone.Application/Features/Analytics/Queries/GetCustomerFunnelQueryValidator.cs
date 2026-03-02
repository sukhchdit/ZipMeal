using FluentValidation;

namespace SwiggyClone.Application.Features.Analytics.Queries;

public sealed class GetCustomerFunnelQueryValidator
    : AbstractValidator<GetCustomerFunnelQuery>
{
    public GetCustomerFunnelQueryValidator()
    {
        RuleFor(x => x.Days)
            .InclusiveBetween(7, 365);
    }
}
