using FluentValidation;

namespace SwiggyClone.Application.Features.Analytics.Queries;

public sealed class GetRevenueForecastQueryValidator
    : AbstractValidator<GetRevenueForecastQuery>
{
    public GetRevenueForecastQueryValidator()
    {
        RuleFor(x => x.Days)
            .InclusiveBetween(14, 365);

        RuleFor(x => x.ForecastDays)
            .InclusiveBetween(1, 90);

        When(x => x.RestaurantId.HasValue, () =>
        {
            RuleFor(x => x.RestaurantId!.Value).NotEmpty();
        });
    }
}
