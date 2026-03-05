using FluentAssertions;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Reviews.Commands;
using SwiggyClone.Domain.Entities;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.Reviews;

public sealed class ReplyToReviewCommandHandlerTests
{
    private readonly IAppDbContext _db;
    private readonly ReplyToReviewCommandHandler _handler;
    private readonly List<Review> _reviews;
    private readonly Restaurant _restaurant;

    public ReplyToReviewCommandHandlerTests()
    {
        _restaurant = new RestaurantBuilder().Build();
        var review = new ReviewBuilder()
            .WithRestaurant(_restaurant)
            .Build();

        _reviews = [review];

        _db = MockDbContextFactory.Create(
            reviews: _reviews,
            restaurants: [_restaurant]);

        _handler = new ReplyToReviewCommandHandler(_db);
    }

    [Fact]
    public async Task Handle_ValidRequest_AddsReply()
    {
        var command = new ReplyToReviewCommand(
            ReviewId: TestConstants.ReviewId,
            OwnerId: TestConstants.OwnerId,
            ReplyText: "Thank you for your feedback!");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _reviews[0].RestaurantReply.Should().Be("Thank you for your feedback!");
        _reviews[0].RepliedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_ReviewNotFound_ReturnsFailure()
    {
        var command = new ReplyToReviewCommand(
            ReviewId: Guid.NewGuid(),
            OwnerId: TestConstants.OwnerId,
            ReplyText: "Thanks!");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("REVIEW_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_NotOwner_ReturnsUnauthorized()
    {
        var command = new ReplyToReviewCommand(
            ReviewId: TestConstants.ReviewId,
            OwnerId: Guid.NewGuid(),
            ReplyText: "Thanks!");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("UNAUTHORIZED");
    }
}
