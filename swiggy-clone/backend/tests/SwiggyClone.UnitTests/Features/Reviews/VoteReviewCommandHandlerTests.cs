using FluentAssertions;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Reviews.Commands.VoteReview;
using SwiggyClone.Domain.Entities;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.Reviews;

public sealed class VoteReviewCommandHandlerTests
{
    private readonly IAppDbContext _db;
    private readonly VoteReviewCommandHandler _handler;
    private readonly List<Review> _reviews;
    private readonly List<ReviewVote> _votes;

    public VoteReviewCommandHandlerTests()
    {
        var review = new ReviewBuilder().Build();
        _reviews = [review];
        _votes = [];

        _db = MockDbContextFactory.Create(
            reviews: _reviews,
            reviewVotes: _votes);

        _handler = new VoteReviewCommandHandler(_db);
    }

    [Fact]
    public async Task Handle_NewVote_AddsVote()
    {
        var command = new VoteReviewCommand(
            ReviewId: TestConstants.ReviewId,
            UserId: Guid.NewGuid(),
            IsHelpful: true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _votes.Should().HaveCount(1);
        _votes[0].IsHelpful.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ExistingVote_UpdatesVote()
    {
        var voterId = Guid.NewGuid();
        _votes.Add(new ReviewVote
        {
            ReviewId = TestConstants.ReviewId,
            UserId = voterId,
            IsHelpful = true,
            CreatedAt = DateTimeOffset.UtcNow,
        });

        var command = new VoteReviewCommand(
            ReviewId: TestConstants.ReviewId,
            UserId: voterId,
            IsHelpful: false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _votes.Should().HaveCount(1);
        _votes[0].IsHelpful.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ReviewNotFound_ReturnsFailure()
    {
        var command = new VoteReviewCommand(
            ReviewId: Guid.NewGuid(),
            UserId: TestConstants.UserId,
            IsHelpful: true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("REVIEW_NOT_FOUND");
    }
}
