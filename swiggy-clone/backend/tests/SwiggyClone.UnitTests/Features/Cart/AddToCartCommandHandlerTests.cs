using FluentAssertions;
using NSubstitute;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Cart.Commands;
using SwiggyClone.Application.Features.Cart.DTOs;
using SwiggyClone.Shared;
using SwiggyClone.UnitTests.Common;

namespace SwiggyClone.UnitTests.Features.Cart;

public sealed class AddToCartCommandHandlerTests
{
    private readonly ICartService _cartService;
    private readonly AddToCartCommandHandler _handler;

    public AddToCartCommandHandlerTests()
    {
        _cartService = Substitute.For<ICartService>();
        _handler = new AddToCartCommandHandler(_cartService);
    }

    [Fact]
    public async Task Handle_ValidItem_ReturnsSuccessWithCart()
    {
        // Arrange
        var cartItems = new List<CartItemDto>
        {
            new("item-1", TestConstants.MenuItemId, null, "Paneer Tikka", null,
                2, 19900, 39800, [], null)
        };
        var expectedCart = new CartDto(TestConstants.RestaurantId, "Test Restaurant", cartItems, 39800);

        _cartService.AddToCartAsync(
                TestConstants.UserId,
                Arg.Any<AddToCartItem>(),
                Arg.Any<CancellationToken>())
            .Returns(Result<CartDto>.Success(expectedCart));

        var command = new AddToCartCommand(
            TestConstants.UserId,
            TestConstants.RestaurantId,
            TestConstants.MenuItemId,
            null,
            2,
            null,
            []);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.RestaurantId.Should().Be(TestConstants.RestaurantId);
        result.Value.Items.Should().HaveCount(1);
        result.Value.Subtotal.Should().Be(39800);
    }

    [Fact]
    public async Task Handle_ValidItem_DelegatesToCartService()
    {
        // Arrange
        var expectedCart = new CartDto(TestConstants.RestaurantId, "Test Restaurant", [], 0);
        _cartService.AddToCartAsync(
                TestConstants.UserId,
                Arg.Any<AddToCartItem>(),
                Arg.Any<CancellationToken>())
            .Returns(Result<CartDto>.Success(expectedCart));

        var command = new AddToCartCommand(
            TestConstants.UserId,
            TestConstants.RestaurantId,
            TestConstants.MenuItemId,
            null,
            1,
            "Extra cheese",
            [new CartAddonInput(Guid.NewGuid(), 1)]);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _cartService.Received(1).AddToCartAsync(
            TestConstants.UserId,
            Arg.Is<AddToCartItem>(item =>
                item.RestaurantId == TestConstants.RestaurantId &&
                item.MenuItemId == TestConstants.MenuItemId &&
                item.Quantity == 1 &&
                item.SpecialInstructions == "Extra cheese" &&
                item.Addons.Count == 1),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ItemNotAvailable_ReturnsFailure()
    {
        // Arrange: cart service returns failure for unavailable item
        _cartService.AddToCartAsync(
                TestConstants.UserId,
                Arg.Any<AddToCartItem>(),
                Arg.Any<CancellationToken>())
            .Returns(Result<CartDto>.Failure("ITEM_NOT_AVAILABLE", "Menu item is not available."));

        var command = new AddToCartCommand(
            TestConstants.UserId,
            TestConstants.RestaurantId,
            TestConstants.MenuItemId,
            null,
            1,
            null,
            []);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("ITEM_NOT_AVAILABLE");
    }

    [Fact]
    public async Task Handle_DifferentRestaurant_ReturnsFailure()
    {
        // Arrange: cart service returns failure when trying to add from different restaurant
        _cartService.AddToCartAsync(
                TestConstants.UserId,
                Arg.Any<AddToCartItem>(),
                Arg.Any<CancellationToken>())
            .Returns(Result<CartDto>.Failure("DIFFERENT_RESTAURANT",
                "Your cart has items from a different restaurant. Clear it first."));

        var command = new AddToCartCommand(
            TestConstants.UserId,
            Guid.NewGuid(),
            TestConstants.MenuItemId,
            null,
            1,
            null,
            []);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("DIFFERENT_RESTAURANT");
    }

    [Fact]
    public async Task Handle_WithVariantAndAddons_PassesAllFieldsToService()
    {
        // Arrange
        var variantId = Guid.NewGuid();
        var addonId = Guid.NewGuid();
        var expectedCart = new CartDto(TestConstants.RestaurantId, "Test Restaurant", [], 0);

        _cartService.AddToCartAsync(
                TestConstants.UserId,
                Arg.Any<AddToCartItem>(),
                Arg.Any<CancellationToken>())
            .Returns(Result<CartDto>.Success(expectedCart));

        var command = new AddToCartCommand(
            TestConstants.UserId,
            TestConstants.RestaurantId,
            TestConstants.MenuItemId,
            variantId,
            3,
            "No onions",
            [new CartAddonInput(addonId, 2)]);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _cartService.Received(1).AddToCartAsync(
            TestConstants.UserId,
            Arg.Is<AddToCartItem>(item =>
                item.VariantId == variantId &&
                item.Quantity == 3 &&
                item.SpecialInstructions == "No onions" &&
                item.Addons.Count == 1 &&
                item.Addons[0].AddonId == addonId &&
                item.Addons[0].Quantity == 2),
            Arg.Any<CancellationToken>());
    }
}
