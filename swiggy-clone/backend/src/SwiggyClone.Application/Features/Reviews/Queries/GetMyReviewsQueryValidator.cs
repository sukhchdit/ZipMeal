using FluentValidation;

namespace SwiggyClone.Application.Features.Reviews.Queries;

public sealed class GetMyReviewsQueryValidator : AbstractValidator<GetMyReviewsQuery>
{
    public GetMyReviewsQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}
