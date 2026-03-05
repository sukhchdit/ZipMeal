using FluentValidation.TestHelper;
using SwiggyClone.Application.Features.Reviews.Commands;

namespace SwiggyClone.UnitTests.Validators.Reviews;

public sealed class ReplyToReviewCommandValidatorTests
{
    private readonly ReplyToReviewCommandValidator _validator = new();

    private static ReplyToReviewCommand ValidCommand() => new(
        ReviewId: Guid.NewGuid(),
        OwnerId: Guid.NewGuid(),
        ReplyText: "Thank you for your feedback!");

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
    public void Validate_EmptyOwnerId_HasError()
    {
        var cmd = ValidCommand() with { OwnerId = Guid.Empty };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.OwnerId);
    }

    [Fact]
    public void Validate_EmptyReplyText_HasError()
    {
        var cmd = ValidCommand() with { ReplyText = "" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.ReplyText);
    }

    [Fact]
    public void Validate_ReplyTextTooLong_HasError()
    {
        var cmd = ValidCommand() with { ReplyText = new string('x', 1001) };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.ReplyText);
    }

    [Fact]
    public void Validate_ReplyTextAtMaxLength_HasNoError()
    {
        var cmd = ValidCommand() with { ReplyText = new string('x', 1000) };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.ReplyText);
    }
}
