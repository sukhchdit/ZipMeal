using FluentValidation.TestHelper;
using SwiggyClone.Application.Features.Recommendations.Commands.TrackInteraction;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.UnitTests.Validators.Recommendations;

public sealed class TrackInteractionCommandValidatorTests
{
    private readonly TrackInteractionCommandValidator _validator = new();

    private static TrackInteractionCommand ValidCommand() => new(
        UserId: Guid.NewGuid(),
        EntityType: InteractionEntityType.Restaurant,
        EntityId: Guid.NewGuid(),
        InteractionType: InteractionType.View);

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var result = _validator.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    // ── UserId ──

    [Fact]
    public void Validate_EmptyUserId_HasError()
    {
        var cmd = ValidCommand() with { UserId = Guid.Empty };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    // ── EntityId ──

    [Fact]
    public void Validate_EmptyEntityId_HasError()
    {
        var cmd = ValidCommand() with { EntityId = Guid.Empty };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.EntityId);
    }

    // ── EntityType ──

    [Fact]
    public void Validate_ValidEntityTypeRestaurant_HasNoError()
    {
        var cmd = ValidCommand() with { EntityType = InteractionEntityType.Restaurant };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.EntityType);
    }

    [Fact]
    public void Validate_ValidEntityTypeMenuItem_HasNoError()
    {
        var cmd = ValidCommand() with { EntityType = InteractionEntityType.MenuItem };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.EntityType);
    }

    [Fact]
    public void Validate_InvalidEntityType_HasError()
    {
        var cmd = ValidCommand() with { EntityType = (InteractionEntityType)999 };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.EntityType);
    }

    // ── InteractionType ──

    [Fact]
    public void Validate_ValidInteractionTypeView_HasNoError()
    {
        var cmd = ValidCommand() with { InteractionType = InteractionType.View };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.InteractionType);
    }

    [Fact]
    public void Validate_ValidInteractionTypeClick_HasNoError()
    {
        var cmd = ValidCommand() with { InteractionType = InteractionType.Click };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.InteractionType);
    }

    [Fact]
    public void Validate_ValidInteractionTypeOrder_HasNoError()
    {
        var cmd = ValidCommand() with { InteractionType = InteractionType.Order };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.InteractionType);
    }

    [Fact]
    public void Validate_ValidInteractionTypeSearch_HasNoError()
    {
        var cmd = ValidCommand() with { InteractionType = InteractionType.Search };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.InteractionType);
    }

    [Fact]
    public void Validate_InvalidInteractionType_HasError()
    {
        var cmd = ValidCommand() with { InteractionType = (InteractionType)999 };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.InteractionType);
    }
}
