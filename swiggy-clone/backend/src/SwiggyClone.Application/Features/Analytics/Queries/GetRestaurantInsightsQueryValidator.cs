using FluentValidation;

namespace SwiggyClone.Application.Features.Analytics.Queries;

public sealed class GetRestaurantInsightsQueryValidator
    : AbstractValidator<GetRestaurantInsightsQuery>
{
    private static readonly string[] ValidPeriods = ["daily", "weekly", "monthly"];

    public GetRestaurantInsightsQueryValidator()
    {
        RuleFor(x => x.RestaurantId).NotEmpty();

        RuleFor(x => x.Period)
            .Must(p => ValidPeriods.Contains(p))
            .WithMessage("Period must be 'daily', 'weekly', or 'monthly'.");

        RuleFor(x => x.Days)
            .InclusiveBetween(7, 365);
    }
}
