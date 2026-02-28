using FluentValidation.TestHelper;
using SwiggyClone.Application.Features.Orders.Commands;

namespace SwiggyClone.UnitTests.Validators.Orders;

public sealed class PlaceOrderCommandValidatorTests
{
    private readonly PlaceOrderCommandValidator _validator = new();

    private static PlaceOrderCommand ValidCommand() =>
        new(Guid.NewGuid(), Guid.NewGuid(), 1, null, null);

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
    public void Validate_EmptyDeliveryAddressId_HasError()
    {
        var cmd = ValidCommand() with { DeliveryAddressId = Guid.Empty };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.DeliveryAddressId);
    }

    [Fact]
    public void Validate_PaymentMethodTooLow_HasError()
    {
        var cmd = ValidCommand() with { PaymentMethod = 0 };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.PaymentMethod);
    }

    [Fact]
    public void Validate_PaymentMethodTooHigh_HasError()
    {
        var cmd = ValidCommand() with { PaymentMethod = 7 };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.PaymentMethod);
    }

    [Fact]
    public void Validate_LongSpecialInstructions_HasError()
    {
        var cmd = ValidCommand() with { SpecialInstructions = new string('x', 501) };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.SpecialInstructions);
    }

    [Fact]
    public void Validate_LongCouponCode_HasError()
    {
        var cmd = ValidCommand() with { CouponCode = new string('X', 21) };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.CouponCode);
    }

    [Fact]
    public void Validate_ValidPaymentMethodBoundary_HasNoErrors()
    {
        var cmd = ValidCommand() with { PaymentMethod = 6 };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.PaymentMethod);
    }
}
