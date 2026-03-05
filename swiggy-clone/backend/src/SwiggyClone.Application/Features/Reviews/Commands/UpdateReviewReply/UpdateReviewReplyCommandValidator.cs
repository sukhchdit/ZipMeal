using FluentValidation;

namespace SwiggyClone.Application.Features.Reviews.Commands.UpdateReviewReply;

public sealed class UpdateReviewReplyCommandValidator : AbstractValidator<UpdateReviewReplyCommand>
{
    public UpdateReviewReplyCommandValidator()
    {
        RuleFor(x => x.ReviewId).NotEmpty();
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.ReplyText).NotEmpty().MaximumLength(1000)
            .WithMessage("Reply must not exceed 1000 characters.");
    }
}
