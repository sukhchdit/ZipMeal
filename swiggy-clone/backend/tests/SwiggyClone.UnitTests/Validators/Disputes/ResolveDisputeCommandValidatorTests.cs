using FluentAssertions;
using FluentValidation.TestHelper;
using SwiggyClone.Application.Features.Disputes.Commands.ResolveDispute;

namespace SwiggyClone.UnitTests.Validators.Disputes;

public sealed class ResolveDisputeCommandValidatorTests
{
    private readonly ResolveDisputeCommandValidator _validator = new();

    private static ResolveDisputeCommand ValidCommand() =>
        new(Guid.NewGuid(), Guid.NewGuid(), 0, 50000, "Full refund approved.");

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var result = _validator.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyAgentId_HasError()
    {
        var cmd = ValidCommand() with { AgentId = Guid.Empty };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.AgentId);
    }

    [Fact]
    public void Validate_EmptyDisputeId_HasError()
    {
        var cmd = ValidCommand() with { DisputeId = Guid.Empty };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.DisputeId);
    }

    [Fact]
    public void Validate_ResolutionTypeNegative_HasError()
    {
        var cmd = ValidCommand() with { ResolutionType = -1 };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.ResolutionType);
    }

    [Fact]
    public void Validate_ResolutionTypeAboveMax_HasError()
    {
        var cmd = ValidCommand() with { ResolutionType = 6 };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.ResolutionType);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void Validate_ResolutionTypeInRange_HasNoError(int resolutionType)
    {
        var cmd = ValidCommand() with { ResolutionType = resolutionType };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.ResolutionType);
    }

    [Theory]
    [InlineData(0)] // FullRefund
    [InlineData(1)] // PartialRefund
    [InlineData(2)] // WalletCredit
    public void Validate_RefundTypeWithoutAmount_HasNoError(int resolutionType)
    {
        // FluentValidation's GreaterThan(0) passes for null values (null is not validated)
        // The validator only enforces > 0 when a value IS provided
        var cmd = ValidCommand() with { ResolutionType = resolutionType, ResolutionAmountPaise = null };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.ResolutionAmountPaise);
    }

    [Theory]
    [InlineData(0)] // FullRefund
    [InlineData(1)] // PartialRefund
    [InlineData(2)] // WalletCredit
    public void Validate_RefundTypeWithZeroAmount_HasError(int resolutionType)
    {
        var cmd = ValidCommand() with { ResolutionType = resolutionType, ResolutionAmountPaise = 0 };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.ResolutionAmountPaise);
    }

    [Theory]
    [InlineData(0)] // FullRefund
    [InlineData(1)] // PartialRefund
    [InlineData(2)] // WalletCredit
    public void Validate_RefundTypeWithNegativeAmount_HasError(int resolutionType)
    {
        var cmd = ValidCommand() with { ResolutionType = resolutionType, ResolutionAmountPaise = -500 };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.ResolutionAmountPaise);
    }

    [Theory]
    [InlineData(3)] // Replacement
    [InlineData(4)] // Coupon
    [InlineData(5)] // NoAction
    public void Validate_NonRefundTypeWithoutAmount_HasNoError(int resolutionType)
    {
        var cmd = ValidCommand() with { ResolutionType = resolutionType, ResolutionAmountPaise = null };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.ResolutionAmountPaise);
    }

    [Fact]
    public void Validate_ResolutionNotesExceedsMaxLength_HasError()
    {
        var cmd = ValidCommand() with { ResolutionNotes = new string('x', 2001) };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.ResolutionNotes);
    }

    [Fact]
    public void Validate_ResolutionNotesAtMaxLength_HasNoError()
    {
        var cmd = ValidCommand() with { ResolutionNotes = new string('x', 2000) };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.ResolutionNotes);
    }

    [Fact]
    public void Validate_NullResolutionNotes_HasNoError()
    {
        var cmd = ValidCommand() with { ResolutionNotes = null };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.ResolutionNotes);
    }

    [Fact]
    public void Validate_RefundTypeWithValidAmount_HasNoError()
    {
        var cmd = ValidCommand() with { ResolutionType = 0, ResolutionAmountPaise = 10000 };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
