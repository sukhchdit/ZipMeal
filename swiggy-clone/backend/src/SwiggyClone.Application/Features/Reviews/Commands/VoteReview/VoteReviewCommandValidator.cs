using FluentValidation;

namespace SwiggyClone.Application.Features.Reviews.Commands.VoteReview;

public sealed class VoteReviewCommandValidator : AbstractValidator<VoteReviewCommand>
{
    public VoteReviewCommandValidator()
    {
        RuleFor(x => x.ReviewId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
