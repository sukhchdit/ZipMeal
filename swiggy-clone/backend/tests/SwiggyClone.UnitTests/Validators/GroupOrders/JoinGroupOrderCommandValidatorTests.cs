using FluentValidation.TestHelper;
using SwiggyClone.Application.Features.GroupOrders.Commands;

namespace SwiggyClone.UnitTests.Validators.GroupOrders;

public sealed class JoinGroupOrderCommandValidatorTests
{
    private readonly JoinGroupOrderCommandValidator _validator = new();

    private static JoinGroupOrderCommand ValidCommand() =>
        new(Guid.NewGuid(), "A3K9X2");

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
    public void Validate_EmptyInviteCode_HasError()
    {
        var cmd = ValidCommand() with { InviteCode = "" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.InviteCode);
    }

    [Fact]
    public void Validate_NullInviteCode_HasError()
    {
        var cmd = ValidCommand() with { InviteCode = null! };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.InviteCode);
    }

    [Fact]
    public void Validate_InviteCodeTooLong_HasError()
    {
        var cmd = ValidCommand() with { InviteCode = "ABCDEFG" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.InviteCode);
    }

    [Fact]
    public void Validate_InviteCodeExactLength_HasNoErrors()
    {
        var cmd = ValidCommand() with { InviteCode = "ABC123" };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShortInviteCode_HasNoErrors()
    {
        var cmd = ValidCommand() with { InviteCode = "AB" };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
