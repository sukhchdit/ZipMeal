using FluentValidation;

namespace SwiggyClone.Application.Features.Reviews.Commands.ReportReview;

public sealed class ReportReviewCommandValidator : AbstractValidator<ReportReviewCommand>
{
    public ReportReviewCommandValidator()
    {
        RuleFor(x => x.ReviewId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Reason).IsInEnum();
        RuleFor(x => x.Description).MaximumLength(1000)
            .When(x => x.Description is not null);
    }
}
