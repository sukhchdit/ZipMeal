using FluentValidation;

namespace SwiggyClone.Application.Features.Discovery.Queries;

public sealed class BrowseRestaurantsQueryValidator : AbstractValidator<BrowseRestaurantsQuery>
{
    public BrowseRestaurantsQueryValidator()
    {
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);

        When(x => x.MinRating.HasValue, () =>
            RuleFor(x => x.MinRating!.Value).InclusiveBetween(0, 5));

        When(x => x.MaxCostForTwo.HasValue, () =>
            RuleFor(x => x.MaxCostForTwo!.Value).GreaterThan(0));

        When(x => !string.IsNullOrWhiteSpace(x.SortBy), () =>
            RuleFor(x => x.SortBy).Must(s =>
                s is "rating" or "deliveryTime" or "costLowToHigh" or "costHighToLow")
                .WithMessage("SortBy must be one of: rating, deliveryTime, costLowToHigh, costHighToLow"));
    }
}
