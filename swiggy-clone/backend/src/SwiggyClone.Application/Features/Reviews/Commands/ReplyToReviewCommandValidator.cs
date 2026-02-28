using FluentValidation;

namespace SwiggyClone.Application.Features.Reviews.Commands;

public sealed class ReplyToReviewCommandValidator : AbstractValidator<ReplyToReviewCommand>
{
    public ReplyToReviewCommandValidator()
    {
        RuleFor(x => x.ReviewId).NotEmpty();
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.ReplyText).NotEmpty().MaximumLength(1000)
            .WithMessage("Reply must not exceed 1000 characters.");
    }
}
