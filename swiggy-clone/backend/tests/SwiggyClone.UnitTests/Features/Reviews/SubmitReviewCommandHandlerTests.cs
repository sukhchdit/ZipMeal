using FluentAssertions;
using MediatR;
using NSubstitute;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Reviews.Commands;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.Reviews;

public sealed class SubmitReviewCommandHandlerTests
{
    private readonly IPublisher _publisher;
    private readonly List<Review> _reviews = [];
    private readonly List<Order> _orders;
    private readonly List<Restaurant> _restaurants;
    private readonly List<User> _users;
    private readonly IAppDbContext _db;
    private readonly SubmitReviewCommandHandler _handler;

    public SubmitReviewCommandHandlerTests()
    {
        _publisher = Substitute.For<IPublisher>();

        var user = new UserBuilder().Build();
        var restaurant = new RestaurantBuilder().Build();
        var order = new OrderBuilder()
            .WithStatus(OrderStatus.Delivered)
            .Build();

        _users = [user];
        _restaurants = [restaurant];
        _orders = [order];

        _db = MockDbContextFactory.Create(
            users: _users,
            reviews: _reviews,
            orders: _orders,
            restaurants: _restaurants);

        _handler = new SubmitReviewCommandHandler(_db, _publisher);
    }

    private static SubmitReviewCommand ValidCommand() => new(
        UserId: TestConstants.UserId,
        OrderId: TestConstants.OrderId,
        Rating: 4,
        ReviewText: "Great food!",
        DeliveryRating: 5,
        IsAnonymous: false,
        PhotoUrls: []);

    [Fact]
    public async Task Handle_ValidRequest_CreatesReview()
    {
        var command = ValidCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Rating.Should().Be(4);
        result.Value.ReviewText.Should().Be("Great food!");
        result.Value.DeliveryRating.Should().Be(5);
        result.Value.RestaurantId.Should().Be(TestConstants.RestaurantId);
        _reviews.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_OrderNotFound_ReturnsFailure()
    {
        var command = ValidCommand() with { OrderId = Guid.NewGuid() };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("ORDER_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_OrderNotDelivered_ReturnsFailure()
    {
        _orders.Clear();
        _orders.Add(new OrderBuilder().WithStatus(OrderStatus.Placed).Build());

        var command = ValidCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("ORDER_NOT_DELIVERED");
    }

    [Fact]
    public async Task Handle_ReviewAlreadyExists_ReturnsFailure()
    {
        _reviews.Add(new ReviewBuilder().Build());

        var command = ValidCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("REVIEW_EXISTS");
    }

    [Fact]
    public async Task Handle_WithPhotos_CreatesReviewWithPhotos()
    {
        var command = ValidCommand() with
        {
            PhotoUrls = ["https://img.example.com/1.jpg", "https://img.example.com/2.jpg"]
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Photos.Should().HaveCount(2);
        result.Value.Photos[0].PhotoUrl.Should().Be("https://img.example.com/1.jpg");
        result.Value.Photos[1].PhotoUrl.Should().Be("https://img.example.com/2.jpg");
    }

    [Fact]
    public async Task Handle_Success_UpdatesRestaurantRating()
    {
        var restaurant = _restaurants[0];
        restaurant.AverageRating = 3.0m;
        restaurant.TotalRatings = 10;

        var command = ValidCommand() with { Rating = 5 };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        restaurant.TotalRatings.Should().Be(11);
        restaurant.AverageRating.Should().BeGreaterThan(3.0m);
    }
}
