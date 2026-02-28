using FluentValidation;

namespace SwiggyClone.Application.Features.Reviews.Commands;

public sealed class SubmitReviewCommandValidator : AbstractValidator<SubmitReviewCommand>
{
    public SubmitReviewCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween((short)1, (short)5)
            .WithMessage("Rating must be between 1 and 5.");
        RuleFor(x => x.ReviewText).MaximumLength(2000)
            .WithMessage("Review text must not exceed 2000 characters.");
        RuleFor(x => x.DeliveryRating).InclusiveBetween((short)1, (short)5)
            .When(x => x.DeliveryRating.HasValue)
            .WithMessage("Delivery rating must be between 1 and 5.");
        RuleFor(x => x.PhotoUrls).Must(urls => urls.Count <= 5)
            .WithMessage("Maximum 5 photos allowed.");
    }
}
