using FluentValidation.TestHelper;
using SwiggyClone.Application.Features.Loyalty.Commands.RedeemReward;

namespace SwiggyClone.UnitTests.Validators.Loyalty;

public sealed class RedeemRewardCommandValidatorTests
{
    private readonly RedeemRewardCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new RedeemRewardCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyUserId_HasValidationError()
    {
        // Arrange
        var command = new RedeemRewardCommand(Guid.Empty, Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_EmptyRewardId_HasValidationError()
    {
        // Arrange
        var command = new RedeemRewardCommand(Guid.NewGuid(), Guid.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RewardId);
    }

    [Fact]
    public void Validate_BothEmpty_HasValidationErrors()
    {
        // Arrange
        var command = new RedeemRewardCommand(Guid.Empty, Guid.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId);
        result.ShouldHaveValidationErrorFor(x => x.RewardId);
    }
}
