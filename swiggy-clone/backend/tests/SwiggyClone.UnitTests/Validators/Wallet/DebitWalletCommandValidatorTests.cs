using FluentValidation.TestHelper;
using SwiggyClone.Application.Features.Wallet.Commands.DebitWallet;

namespace SwiggyClone.UnitTests.Validators.Wallet;

public sealed class DebitWalletCommandValidatorTests
{
    private readonly DebitWalletCommandValidator _validator = new();

    private static DebitWalletCommand ValidCommand() =>
        new(Guid.NewGuid(), 5000, null, "Order payment");

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
    public void Validate_ZeroAmount_HasError()
    {
        var cmd = ValidCommand() with { AmountPaise = 0 };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.AmountPaise);
    }

    [Fact]
    public void Validate_EmptyDescription_HasError()
    {
        var cmd = ValidCommand() with { Description = "" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_LongDescription_HasError()
    {
        var cmd = ValidCommand() with { Description = new string('x', 501) };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
