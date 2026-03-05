using FluentAssertions;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Reviews.Commands.DeleteReviewReply;
using SwiggyClone.Domain.Entities;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.Reviews;

public sealed class DeleteReviewReplyCommandHandlerTests
{
    private readonly IAppDbContext _db;
    private readonly DeleteReviewReplyCommandHandler _handler;
    private readonly List<Review> _reviews;

    public DeleteReviewReplyCommandHandlerTests()
    {
        var restaurant = new RestaurantBuilder().Build();
        var review = new ReviewBuilder()
            .WithRestaurant(restaurant)
            .WithRestaurantReply("Thank you!")
            .WithRepliedAt(DateTimeOffset.UtcNow.AddDays(-1))
            .Build();

        _reviews = [review];

        _db = MockDbContextFactory.Create(
            reviews: _reviews,
            restaurants: [restaurant]);

        _handler = new DeleteReviewReplyCommandHandler(_db);
    }

    [Fact]
    public async Task Handle_ValidRequest_DeletesReply()
    {
        var command = new DeleteReviewReplyCommand(
            ReviewId: TestConstants.ReviewId,
            OwnerId: TestConstants.OwnerId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _reviews[0].RestaurantReply.Should().BeNull();
        _reviews[0].RepliedAt.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ReviewNotFound_ReturnsFailure()
    {
        var command = new DeleteReviewReplyCommand(
            ReviewId: Guid.NewGuid(),
            OwnerId: TestConstants.OwnerId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("REVIEW_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_NotOwner_ReturnsUnauthorized()
    {
        var command = new DeleteReviewReplyCommand(
            ReviewId: TestConstants.ReviewId,
            OwnerId: Guid.NewGuid());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("UNAUTHORIZED");
    }
}
