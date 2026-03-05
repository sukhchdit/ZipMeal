using FluentValidation.TestHelper;
using SwiggyClone.Application.Features.Reviews.Commands.ReportReview;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.UnitTests.Validators.Reviews;

public sealed class ReportReviewCommandValidatorTests
{
    private readonly ReportReviewCommandValidator _validator = new();

    private static ReportReviewCommand ValidCommand() => new(
        ReviewId: Guid.NewGuid(),
        UserId: Guid.NewGuid(),
        Reason: ReviewReportReason.Spam,
        Description: "This is spam content");

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var result = _validator.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyReviewId_HasError()
    {
        var cmd = ValidCommand() with { ReviewId = Guid.Empty };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.ReviewId);
    }

    [Fact]
    public void Validate_EmptyUserId_HasError()
    {
        var cmd = ValidCommand() with { UserId = Guid.Empty };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_InvalidReason_HasError()
    {
        var cmd = ValidCommand() with { Reason = (ReviewReportReason)99 };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Reason);
    }

    [Fact]
    public void Validate_DescriptionTooLong_HasError()
    {
        var cmd = ValidCommand() with { Description = new string('x', 1001) };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_NullDescription_HasNoError()
    {
        var cmd = ValidCommand() with { Description = null };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_DescriptionAtMaxLength_HasNoError()
    {
        var cmd = ValidCommand() with { Description = new string('x', 1000) };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData(ReviewReportReason.Spam)]
    [InlineData(ReviewReportReason.Inappropriate)]
    [InlineData(ReviewReportReason.FakeReview)]
    [InlineData(ReviewReportReason.Harassment)]
    [InlineData(ReviewReportReason.Other)]
    public void Validate_AllValidReasons_HasNoError(ReviewReportReason reason)
    {
        var cmd = ValidCommand() with { Reason = reason };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.Reason);
    }
}
