using FluentValidation.TestHelper;
using SwiggyClone.Application.Features.Loyalty.Commands.EarnPoints;

namespace SwiggyClone.UnitTests.Validators.Loyalty;

public sealed class EarnPointsCommandValidatorTests
{
    private readonly EarnPointsCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new EarnPointsCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyUserId_HasValidationError()
    {
        // Arrange
        var command = new EarnPointsCommand(Guid.Empty, Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_EmptyOrderId_HasValidationError()
    {
        // Arrange
        var command = new EarnPointsCommand(Guid.NewGuid(), Guid.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OrderId);
    }

    [Fact]
    public void Validate_BothEmpty_HasValidationErrors()
    {
        // Arrange
        var command = new EarnPointsCommand(Guid.Empty, Guid.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId);
        result.ShouldHaveValidationErrorFor(x => x.OrderId);
    }
}
