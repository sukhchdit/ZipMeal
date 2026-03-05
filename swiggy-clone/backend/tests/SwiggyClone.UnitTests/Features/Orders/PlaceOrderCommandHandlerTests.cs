using FluentAssertions;
using MediatR;
using NSubstitute;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Cart.DTOs;
using SwiggyClone.Application.Features.Orders.Commands;
using SwiggyClone.Application.Features.Orders.Notifications;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.Orders;

public sealed class PlaceOrderCommandHandlerTests
{
    private readonly IAppDbContext _db;
    private readonly ICartService _cartService;
    private readonly IPublisher _publisher;
    private readonly PlaceOrderCommandHandler _handler;

    private readonly List<Order> _orders = [];
    private readonly List<Payment> _payments = [];
    private readonly List<CouponUsage> _couponUsages = [];
    private readonly List<UserAddress> _userAddresses = [];
    private readonly List<MenuItem> _menuItems = [];
    private readonly List<Restaurant> _restaurants = [];
    private readonly List<PlatformConfig> _platformConfigs = [];
    private readonly List<UserSubscription> _userSubscriptions = [];
    private readonly List<Coupon> _coupons = [];

    public PlaceOrderCommandHandlerTests()
    {
        _db = MockDbContextFactory.Create(
            orders: _orders,
            userAddresses: _userAddresses,
            menuItems: _menuItems,
            restaurants: _restaurants,
            platformConfigs: _platformConfigs,
            userSubscriptions: _userSubscriptions,
            coupons: _coupons,
            couponUsages: _couponUsages);

        _cartService = Substitute.For<ICartService>();
        _publisher = Substitute.For<IPublisher>();
        _handler = new PlaceOrderCommandHandler(_db, _cartService, _publisher);
    }

    [Fact]
    public async Task Handle_CartEmpty_ReturnsFailure()
    {
        // Arrange: cart service returns empty cart
        var emptyCart = new CartDto(TestConstants.RestaurantId, "Test Restaurant", [], 0);
        _cartService.GetCartAsync(TestConstants.UserId, Arg.Any<CancellationToken>())
            .Returns(Result<CartDto>.Success(emptyCart));

        var command = new PlaceOrderCommand(
            TestConstants.UserId, TestConstants.AddressId, 1, null, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("CART_EMPTY");
    }

    [Fact]
    public async Task Handle_CartServiceFails_ReturnsFailure()
    {
        // Arrange: cart service itself returns a failure
        _cartService.GetCartAsync(TestConstants.UserId, Arg.Any<CancellationToken>())
            .Returns(Result<CartDto>.Failure("CART_ERROR", "Redis unavailable."));

        var command = new PlaceOrderCommand(
            TestConstants.UserId, TestConstants.AddressId, 1, null, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("CART_ERROR");
    }

    [Fact]
    public async Task Handle_RestaurantNotFound_ReturnsFailure()
    {
        // Arrange: cart has items but restaurant doesn't exist in DB
        var cartItems = new List<CartItemDto>
        {
            new("item-1", TestConstants.MenuItemId, null, "Paneer Tikka", null,
                1, 19900, 19900, [], null)
        };
        var cart = new CartDto(TestConstants.RestaurantId, "Ghost Restaurant", cartItems, 19900);

        _cartService.GetCartAsync(TestConstants.UserId, Arg.Any<CancellationToken>())
            .Returns(Result<CartDto>.Success(cart));
        // Restaurants list is empty -- restaurant not found

        var command = new PlaceOrderCommand(
            TestConstants.UserId, TestConstants.AddressId, 1, null, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("RESTAURANT_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_RestaurantNotAcceptingOrders_ReturnsFailure()
    {
        // Arrange: restaurant exists but IsAcceptingOrders = false
        var restaurant = new RestaurantBuilder().Build();
        restaurant.IsAcceptingOrders = false;
        _restaurants.Add(restaurant);

        var cartItems = new List<CartItemDto>
        {
            new("item-1", TestConstants.MenuItemId, null, "Paneer Tikka", null,
                1, 19900, 19900, [], null)
        };
        var cart = new CartDto(TestConstants.RestaurantId, restaurant.Name, cartItems, 19900);

        _cartService.GetCartAsync(TestConstants.UserId, Arg.Any<CancellationToken>())
            .Returns(Result<CartDto>.Success(cart));

        var command = new PlaceOrderCommand(
            TestConstants.UserId, TestConstants.AddressId, 1, null, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("RESTAURANT_CLOSED");
    }

    [Fact]
    public async Task Handle_AddressNotFound_ReturnsFailure()
    {
        // Arrange: restaurant is OK but address doesn't exist
        var restaurant = new RestaurantBuilder().Build();
        _restaurants.Add(restaurant);

        var cartItems = new List<CartItemDto>
        {
            new("item-1", TestConstants.MenuItemId, null, "Paneer Tikka", null,
                1, 19900, 19900, [], null)
        };
        var cart = new CartDto(TestConstants.RestaurantId, restaurant.Name, cartItems, 19900);

        _cartService.GetCartAsync(TestConstants.UserId, Arg.Any<CancellationToken>())
            .Returns(Result<CartDto>.Success(cart));
        // UserAddresses list is empty -- address not found

        var command = new PlaceOrderCommand(
            TestConstants.UserId, TestConstants.AddressId, 1, null, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("ADDRESS_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_MenuItemUnavailable_ReturnsFailure()
    {
        // Arrange: menu item is not available
        var restaurant = new RestaurantBuilder().Build();
        _restaurants.Add(restaurant);

        var address = new UserAddress
        {
            Id = TestConstants.AddressId,
            UserId = TestConstants.UserId,
            Label = "Home",
            AddressLine1 = "123 Test St",
            Latitude = 19.076,
            Longitude = 72.877,
            IsDefault = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        _userAddresses.Add(address);

        var menuItem = new MenuItemBuilder().WithIsAvailable(false).Build();
        _menuItems.Add(menuItem);

        var cartItems = new List<CartItemDto>
        {
            new("item-1", TestConstants.MenuItemId, null, "Paneer Tikka", null,
                1, 19900, 19900, [], null)
        };
        var cart = new CartDto(TestConstants.RestaurantId, restaurant.Name, cartItems, 19900);

        _cartService.GetCartAsync(TestConstants.UserId, Arg.Any<CancellationToken>())
            .Returns(Result<CartDto>.Success(cart));

        var command = new PlaceOrderCommand(
            TestConstants.UserId, TestConstants.AddressId, 1, null, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("ITEM_UNAVAILABLE");
    }

    [Fact]
    public async Task Handle_ValidOrder_ReturnsSuccessAndCreatesOrder()
    {
        // Arrange
        var restaurant = new RestaurantBuilder().Build();
        _restaurants.Add(restaurant);

        var address = new UserAddress
        {
            Id = TestConstants.AddressId,
            UserId = TestConstants.UserId,
            Label = "Home",
            AddressLine1 = "123 Test St",
            Latitude = 19.076,
            Longitude = 72.877,
            IsDefault = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        _userAddresses.Add(address);

        var menuItem = new MenuItemBuilder().WithPrice(20000).Build();
        _menuItems.Add(menuItem);

        var cartItems = new List<CartItemDto>
        {
            new("item-1", TestConstants.MenuItemId, null, menuItem.Name, null,
                2, 20000, 40000, [], null)
        };
        var cart = new CartDto(TestConstants.RestaurantId, restaurant.Name, cartItems, 40000);

        _cartService.GetCartAsync(TestConstants.UserId, Arg.Any<CancellationToken>())
            .Returns(Result<CartDto>.Success(cart));
        _cartService.ClearCartAsync(TestConstants.UserId, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var command = new PlaceOrderCommand(
            TestConstants.UserId, TestConstants.AddressId, 1, "No onions please", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.RestaurantId.Should().Be(TestConstants.RestaurantId);
        result.Value.Status.Should().Be(OrderStatus.Placed);
        result.Value.SpecialInstructions.Should().Be("No onions please");
        result.Value.Items.Should().HaveCount(1);
        _orders.Should().HaveCount(1);
        await _db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidOrder_ClearsCartAfterPlacement()
    {
        // Arrange
        var restaurant = new RestaurantBuilder().Build();
        _restaurants.Add(restaurant);

        var address = new UserAddress
        {
            Id = TestConstants.AddressId,
            UserId = TestConstants.UserId,
            Label = "Home",
            AddressLine1 = "123 Test St",
            Latitude = 19.076,
            Longitude = 72.877,
            IsDefault = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        _userAddresses.Add(address);

        var menuItem = new MenuItemBuilder().Build();
        _menuItems.Add(menuItem);

        var cartItems = new List<CartItemDto>
        {
            new("item-1", TestConstants.MenuItemId, null, menuItem.Name, null,
                1, 19900, 19900, [], null)
        };
        var cart = new CartDto(TestConstants.RestaurantId, restaurant.Name, cartItems, 19900);

        _cartService.GetCartAsync(TestConstants.UserId, Arg.Any<CancellationToken>())
            .Returns(Result<CartDto>.Success(cart));
        _cartService.ClearCartAsync(TestConstants.UserId, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var command = new PlaceOrderCommand(
            TestConstants.UserId, TestConstants.AddressId, 1, null, null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _cartService.Received(1).ClearCartAsync(TestConstants.UserId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidOrder_PublishesOrderPlacedNotification()
    {
        // Arrange
        var restaurant = new RestaurantBuilder().Build();
        _restaurants.Add(restaurant);

        var address = new UserAddress
        {
            Id = TestConstants.AddressId,
            UserId = TestConstants.UserId,
            Label = "Home",
            AddressLine1 = "123 Test St",
            Latitude = 19.076,
            Longitude = 72.877,
            IsDefault = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        _userAddresses.Add(address);

        var menuItem = new MenuItemBuilder().Build();
        _menuItems.Add(menuItem);

        var cartItems = new List<CartItemDto>
        {
            new("item-1", TestConstants.MenuItemId, null, menuItem.Name, null,
                1, 19900, 19900, [], null)
        };
        var cart = new CartDto(TestConstants.RestaurantId, restaurant.Name, cartItems, 19900);

        _cartService.GetCartAsync(TestConstants.UserId, Arg.Any<CancellationToken>())
            .Returns(Result<CartDto>.Success(cart));
        _cartService.ClearCartAsync(TestConstants.UserId, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var command = new PlaceOrderCommand(
            TestConstants.UserId, TestConstants.AddressId, 1, null, null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _publisher.Received(1).Publish(
            Arg.Any<OrderPlacedNotification>(), Arg.Any<CancellationToken>());
    }
}
