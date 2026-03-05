using FluentAssertions;
using FluentValidation.TestHelper;
using SwiggyClone.Application.Features.Disputes.Commands.CreateDispute;

namespace SwiggyClone.UnitTests.Validators.Disputes;

public sealed class CreateDisputeCommandValidatorTests
{
    private readonly CreateDisputeCommandValidator _validator = new();

    private static CreateDisputeCommand ValidCommand() =>
        new(Guid.NewGuid(), Guid.NewGuid(), 1, "Items were missing from my order.");

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
    public void Validate_IssueTypeNegative_HasError()
    {
        var cmd = ValidCommand() with { IssueType = -1 };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.IssueType);
    }

    [Fact]
    public void Validate_IssueTypeAboveMax_HasError()
    {
        var cmd = ValidCommand() with { IssueType = 8 };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.IssueType);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(7)]
    public void Validate_IssueTypeInRange_HasNoError(int issueType)
    {
        var cmd = ValidCommand() with { IssueType = issueType };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.IssueType);
    }

    [Fact]
    public void Validate_EmptyDescription_HasError()
    {
        var cmd = ValidCommand() with { Description = "" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_NullDescription_HasError()
    {
        var cmd = ValidCommand() with { Description = null! };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_DescriptionExceedsMaxLength_HasError()
    {
        var cmd = ValidCommand() with { Description = new string('x', 2001) };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_DescriptionAtMaxLength_HasNoError()
    {
        var cmd = ValidCommand() with { Description = new string('x', 2000) };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_IssueTypeBoundaryLow_HasNoError()
    {
        var cmd = ValidCommand() with { IssueType = 0 };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.IssueType);
    }

    [Fact]
    public void Validate_IssueTypeBoundaryHigh_HasNoError()
    {
        var cmd = ValidCommand() with { IssueType = 7 };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.IssueType);
    }
}
