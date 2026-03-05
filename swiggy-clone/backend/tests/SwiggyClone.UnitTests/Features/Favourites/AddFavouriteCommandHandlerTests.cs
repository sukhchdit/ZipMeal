using FluentAssertions;
using MediatR;
using NSubstitute;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Favourites.Commands;
using SwiggyClone.Application.Features.Favourites.Notifications;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.Favourites;

public sealed class AddFavouriteCommandHandlerTests
{
    private readonly IAppDbContext _db;
    private readonly IPublisher _publisher;
    private readonly AddFavouriteCommandHandler _handler;

    private readonly List<Restaurant> _restaurants = [];
    private readonly List<UserFavorite> _userFavorites = [];

    public AddFavouriteCommandHandlerTests()
    {
        _db = MockDbContextFactory.Create(
            restaurants: _restaurants,
            userFavorites: _userFavorites);

        _publisher = Substitute.For<IPublisher>();
        _handler = new AddFavouriteCommandHandler(_db, _publisher);
    }

    [Fact]
    public async Task Handle_RestaurantNotFound_ReturnsFailure()
    {
        // Arrange: no restaurants in DB
        var command = new AddFavouriteCommand(TestConstants.UserId, TestConstants.RestaurantId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("RESTAURANT_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_RestaurantNotApproved_ReturnsFailure()
    {
        // Arrange: restaurant exists but is Pending status (not Approved)
        var restaurant = new RestaurantBuilder()
            .WithStatus(RestaurantStatus.Pending)
            .Build();
        _restaurants.Add(restaurant);

        var command = new AddFavouriteCommand(TestConstants.UserId, TestConstants.RestaurantId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("RESTAURANT_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_AlreadyFavourited_ReturnsSuccessIdempotent()
    {
        // Arrange: restaurant is approved and user already favourited it
        var restaurant = new RestaurantBuilder().Build();
        _restaurants.Add(restaurant);

        _userFavorites.Add(new UserFavorite
        {
            UserId = TestConstants.UserId,
            RestaurantId = TestConstants.RestaurantId,
            CreatedAt = DateTimeOffset.UtcNow,
        });

        var command = new AddFavouriteCommand(TestConstants.UserId, TestConstants.RestaurantId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert: idempotent -- returns success but doesn't add a duplicate
        result.IsSuccess.Should().BeTrue();
        _userFavorites.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ValidFavourite_ReturnsSuccessAndAddsRecord()
    {
        // Arrange: restaurant is approved, not yet favourited
        var restaurant = new RestaurantBuilder().Build();
        _restaurants.Add(restaurant);

        var command = new AddFavouriteCommand(TestConstants.UserId, TestConstants.RestaurantId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _userFavorites.Should().HaveCount(1);
        _userFavorites[0].UserId.Should().Be(TestConstants.UserId);
        _userFavorites[0].RestaurantId.Should().Be(TestConstants.RestaurantId);
        await _db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidFavourite_PublishesNotification()
    {
        // Arrange
        var restaurant = new RestaurantBuilder().Build();
        _restaurants.Add(restaurant);

        var command = new AddFavouriteCommand(TestConstants.UserId, TestConstants.RestaurantId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _publisher.Received(1).Publish(
            Arg.Is<RestaurantFavouritedNotification>(n =>
                n.UserId == TestConstants.UserId &&
                n.RestaurantId == TestConstants.RestaurantId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AlreadyFavourited_DoesNotPublishNotification()
    {
        // Arrange: already favourited (idempotent path)
        var restaurant = new RestaurantBuilder().Build();
        _restaurants.Add(restaurant);

        _userFavorites.Add(new UserFavorite
        {
            UserId = TestConstants.UserId,
            RestaurantId = TestConstants.RestaurantId,
            CreatedAt = DateTimeOffset.UtcNow,
        });

        var command = new AddFavouriteCommand(TestConstants.UserId, TestConstants.RestaurantId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert: should NOT publish since nothing changed
        await _publisher.DidNotReceive().Publish(
            Arg.Any<RestaurantFavouritedNotification>(), Arg.Any<CancellationToken>());
    }
}
