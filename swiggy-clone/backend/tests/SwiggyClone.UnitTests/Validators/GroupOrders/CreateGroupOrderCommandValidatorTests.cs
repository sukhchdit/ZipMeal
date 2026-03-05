using FluentValidation.TestHelper;
using SwiggyClone.Application.Features.GroupOrders.Commands;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.UnitTests.Validators.GroupOrders;

public sealed class CreateGroupOrderCommandValidatorTests
{
    private readonly CreateGroupOrderCommandValidator _validator = new();

    private static CreateGroupOrderCommand ValidCommand() =>
        new(Guid.NewGuid(), Guid.NewGuid(), PaymentSplitType.SplitEqual, Guid.NewGuid());

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
    public void Validate_EmptyRestaurantId_HasError()
    {
        var cmd = ValidCommand() with { RestaurantId = Guid.Empty };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.RestaurantId);
    }

    [Fact]
    public void Validate_InvalidPaymentSplitType_HasError()
    {
        var cmd = ValidCommand() with { PaymentSplitType = (PaymentSplitType)99 };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.PaymentSplitType);
    }

    [Fact]
    public void Validate_InitiatorPays_HasNoErrors()
    {
        var cmd = ValidCommand() with { PaymentSplitType = PaymentSplitType.InitiatorPays };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_PayYourShare_HasNoErrors()
    {
        var cmd = ValidCommand() with { PaymentSplitType = PaymentSplitType.PayYourShare };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_NullDeliveryAddressId_HasNoErrors()
    {
        var cmd = ValidCommand() with { DeliveryAddressId = null };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
