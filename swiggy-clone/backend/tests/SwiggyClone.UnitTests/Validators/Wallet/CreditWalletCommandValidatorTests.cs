using FluentValidation.TestHelper;
using SwiggyClone.Application.Features.Wallet.Commands.CreditWallet;

namespace SwiggyClone.UnitTests.Validators.Wallet;

public sealed class CreditWalletCommandValidatorTests
{
    private readonly CreditWalletCommandValidator _validator = new();

    private static CreditWalletCommand ValidCommand() =>
        new(Guid.NewGuid(), 5000, 0, null, "Add money");

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
    public void Validate_NegativeAmount_HasError()
    {
        var cmd = ValidCommand() with { AmountPaise = -100 };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.AmountPaise);
    }

    [Fact]
    public void Validate_SourceOutOfRange_HasError()
    {
        var cmd = ValidCommand() with { Source = 10 };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Source);
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
