using FluentAssertions;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Promotions.Commands;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.Promotions;

public sealed class CreatePromotionCommandHandlerTests
{
    private readonly IAppDbContext _db;
    private readonly CreatePromotionCommandHandler _handler;

    private readonly List<Restaurant> _restaurants = [];
    private readonly List<MenuItem> _menuItems = [];
    private readonly List<RestaurantPromotion> _promotions = [];
    private readonly List<PromotionMenuItem> _promotionMenuItems = [];

    public CreatePromotionCommandHandlerTests()
    {
        _db = MockDbContextFactory.Create(
            restaurants: _restaurants,
            menuItems: _menuItems,
            restaurantPromotions: _promotions,
            promotionMenuItems: _promotionMenuItems);

        _handler = new CreatePromotionCommandHandler(_db);
    }

    private static CreatePromotionCommand CreateValidCommand(
        Guid ownerId,
        List<CreatePromotionMenuItemInput>? menuItemInputs = null) => new(
        ownerId,
        "50% Off Weekend Special",
        "Enjoy 50% off on selected items this weekend",
        "https://cdn.example.com/promo.jpg",
        PromotionType.FlashDeal,
        DiscountType.Percentage,
        50,
        25000, // Max discount 250
        10000, // Min order 100
        DateTimeOffset.UtcNow,
        DateTimeOffset.UtcNow.AddDays(7),
        1,
        null,
        null,
        null,
        null,
        menuItemInputs ?? [new CreatePromotionMenuItemInput(TestConstants.MenuItemId, 1)]);

    [Fact]
    public async Task Handle_RestaurantNotFound_ReturnsFailure()
    {
        // Arrange: no restaurant with this owner
        var command = CreateValidCommand(TestConstants.OwnerId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("RESTAURANT_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_NotOwner_ReturnsFailure()
    {
        // Arrange: restaurant belongs to a different owner
        var restaurant = new RestaurantBuilder()
            .WithOwnerId(Guid.NewGuid())
            .Build();
        _restaurants.Add(restaurant);

        var command = CreateValidCommand(TestConstants.OwnerId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("RESTAURANT_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_MenuItemNotInRestaurant_ReturnsFailure()
    {
        // Arrange: restaurant exists but menu item belongs to different restaurant
        var restaurant = new RestaurantBuilder().Build();
        _restaurants.Add(restaurant);

        var differentRestaurantId = Guid.NewGuid();
        var menuItem = new MenuItemBuilder()
            .WithRestaurantId(differentRestaurantId)
            .Build();
        _menuItems.Add(menuItem);

        var command = CreateValidCommand(TestConstants.OwnerId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("MENU_ITEM_NOT_IN_RESTAURANT");
    }

    [Fact]
    public async Task Handle_MenuItemDoesNotExist_ReturnsFailure()
    {
        // Arrange: restaurant exists but menu item ID doesn't exist at all
        var restaurant = new RestaurantBuilder().Build();
        _restaurants.Add(restaurant);
        // No menu items in DB

        var command = CreateValidCommand(TestConstants.OwnerId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("MENU_ITEM_NOT_IN_RESTAURANT");
    }

    [Fact]
    public async Task Handle_ValidPromotion_ReturnsSuccessWithId()
    {
        // Arrange
        var restaurant = new RestaurantBuilder().Build();
        _restaurants.Add(restaurant);

        var menuItem = new MenuItemBuilder().Build();
        _menuItems.Add(menuItem);

        var command = CreateValidCommand(TestConstants.OwnerId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _promotions.Should().HaveCount(1);
        _promotionMenuItems.Should().HaveCount(1);
        await _db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidPromotion_SetsCorrectFields()
    {
        // Arrange
        var restaurant = new RestaurantBuilder().Build();
        _restaurants.Add(restaurant);

        var menuItem = new MenuItemBuilder().Build();
        _menuItems.Add(menuItem);

        var command = CreateValidCommand(TestConstants.OwnerId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var promotion = _promotions[0];
        promotion.RestaurantId.Should().Be(TestConstants.RestaurantId);
        promotion.Title.Should().Be("50% Off Weekend Special");
        promotion.PromotionType.Should().Be(PromotionType.FlashDeal);
        promotion.DiscountType.Should().Be(DiscountType.Percentage);
        promotion.DiscountValue.Should().Be(50);
        promotion.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_MultipleMenuItems_CreatesAllPromotionMenuItems()
    {
        // Arrange
        var restaurant = new RestaurantBuilder().Build();
        _restaurants.Add(restaurant);

        var menuItem1Id = Guid.NewGuid();
        var menuItem2Id = Guid.NewGuid();

        _menuItems.Add(new MenuItemBuilder()
            .WithId(menuItem1Id)
            .WithName("Burger")
            .Build());
        _menuItems.Add(new MenuItemBuilder()
            .WithId(menuItem2Id)
            .WithName("Fries")
            .Build());

        var menuItemInputs = new List<CreatePromotionMenuItemInput>
        {
            new(menuItem1Id, 1),
            new(menuItem2Id, 2),
        };

        var command = CreateValidCommand(TestConstants.OwnerId, menuItemInputs);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _promotionMenuItems.Should().HaveCount(2);
        _promotionMenuItems.Should().Contain(pmi => pmi.MenuItemId == menuItem1Id && pmi.Quantity == 1);
        _promotionMenuItems.Should().Contain(pmi => pmi.MenuItemId == menuItem2Id && pmi.Quantity == 2);
    }
}
