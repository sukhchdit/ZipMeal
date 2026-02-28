using FluentValidation;

namespace SwiggyClone.Application.Features.Reviews.Commands;

public sealed class ToggleReviewVisibilityCommandValidator : AbstractValidator<ToggleReviewVisibilityCommand>
{
    public ToggleReviewVisibilityCommandValidator()
    {
        RuleFor(x => x.ReviewId).NotEmpty();
    }
}
