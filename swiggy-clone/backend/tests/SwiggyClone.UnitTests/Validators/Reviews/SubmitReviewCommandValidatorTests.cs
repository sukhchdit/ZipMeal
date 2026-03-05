using FluentValidation.TestHelper;
using SwiggyClone.Application.Features.Reviews.Commands;

namespace SwiggyClone.UnitTests.Validators.Reviews;

public sealed class SubmitReviewCommandValidatorTests
{
    private readonly SubmitReviewCommandValidator _validator = new();

    private static SubmitReviewCommand ValidCommand() => new(
        UserId: Guid.NewGuid(),
        OrderId: Guid.NewGuid(),
        Rating: 4,
        ReviewText: "Great food!",
        DeliveryRating: 5,
        IsAnonymous: false,
        PhotoUrls: []);

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var result = _validator.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyUserId_HasError()
    {
        var cmd = ValidCommand() with { UserId = Guid.Empty };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_EmptyOrderId_HasError()
    {
        var cmd = ValidCommand() with { OrderId = Guid.Empty };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.OrderId);
    }

    [Fact]
    public void Validate_RatingBelowRange_HasError()
    {
        var cmd = ValidCommand() with { Rating = 0 };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Rating);
    }

    [Fact]
    public void Validate_RatingAboveRange_HasError()
    {
        var cmd = ValidCommand() with { Rating = 6 };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Rating);
    }

    [Fact]
    public void Validate_ReviewTextTooLong_HasError()
    {
        var cmd = ValidCommand() with { ReviewText = new string('x', 2001) };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.ReviewText);
    }

    [Fact]
    public void Validate_DeliveryRatingBelowRange_HasError()
    {
        var cmd = ValidCommand() with { DeliveryRating = 0 };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.DeliveryRating);
    }

    [Fact]
    public void Validate_DeliveryRatingAboveRange_HasError()
    {
        var cmd = ValidCommand() with { DeliveryRating = 6 };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.DeliveryRating);
    }

    [Fact]
    public void Validate_NullDeliveryRating_HasNoError()
    {
        var cmd = ValidCommand() with { DeliveryRating = null };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.DeliveryRating);
    }

    [Fact]
    public void Validate_TooManyPhotos_HasError()
    {
        var cmd = ValidCommand() with
        {
            PhotoUrls = ["a.jpg", "b.jpg", "c.jpg", "d.jpg", "e.jpg", "f.jpg"]
        };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.PhotoUrls);
    }

    [Fact]
    public void Validate_FivePhotos_HasNoError()
    {
        var cmd = ValidCommand() with
        {
            PhotoUrls = ["a.jpg", "b.jpg", "c.jpg", "d.jpg", "e.jpg"]
        };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.PhotoUrls);
    }
}
