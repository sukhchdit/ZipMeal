using FluentValidation;

namespace SwiggyClone.Application.Features.Reviews.Queries;

public sealed class GetRestaurantReviewsQueryValidator : AbstractValidator<GetRestaurantReviewsQuery>
{
    public GetRestaurantReviewsQueryValidator()
    {
        RuleFor(x => x.RestaurantId).NotEmpty();
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}
