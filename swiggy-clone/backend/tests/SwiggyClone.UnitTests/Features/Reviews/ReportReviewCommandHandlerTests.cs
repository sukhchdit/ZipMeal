using FluentAssertions;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Reviews.Commands.ReportReview;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.Reviews;

public sealed class ReportReviewCommandHandlerTests
{
    private static readonly Guid ReporterId = Guid.Parse("11111111-2222-3333-4444-555555555555");

    private readonly IAppDbContext _db;
    private readonly ReportReviewCommandHandler _handler;
    private readonly List<Review> _reviews;
    private readonly List<ReviewReport> _reports;

    public ReportReviewCommandHandlerTests()
    {
        var review = new ReviewBuilder().Build();
        _reviews = [review];
        _reports = [];

        _db = MockDbContextFactory.Create(
            reviews: _reviews,
            reviewReports: _reports);

        _handler = new ReportReviewCommandHandler(_db);
    }

    [Fact]
    public async Task Handle_ValidReport_CreatesReport()
    {
        var command = new ReportReviewCommand(
            ReviewId: TestConstants.ReviewId,
            UserId: ReporterId,
            Reason: ReviewReportReason.Spam,
            Description: "This is spam");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _reports.Should().HaveCount(1);
        _reports[0].Reason.Should().Be(ReviewReportReason.Spam);
        _reports[0].Status.Should().Be(ReviewReportStatus.Pending);
        _reviews[0].ReportCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ReviewNotFound_ReturnsFailure()
    {
        var command = new ReportReviewCommand(
            ReviewId: Guid.NewGuid(),
            UserId: ReporterId,
            Reason: ReviewReportReason.Spam,
            Description: null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("REVIEW_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_OwnReview_ReturnsFailure()
    {
        var command = new ReportReviewCommand(
            ReviewId: TestConstants.ReviewId,
            UserId: TestConstants.UserId,
            Reason: ReviewReportReason.FakeReview,
            Description: null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("CANNOT_REPORT_OWN_REVIEW");
    }

    [Fact]
    public async Task Handle_AlreadyReported_ReturnsFailure()
    {
        _reports.Add(new ReviewReport
        {
            Id = Guid.NewGuid(),
            ReviewId = TestConstants.ReviewId,
            UserId = ReporterId,
            Reason = ReviewReportReason.Spam,
            Status = ReviewReportStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
        });

        var command = new ReportReviewCommand(
            ReviewId: TestConstants.ReviewId,
            UserId: ReporterId,
            Reason: ReviewReportReason.Spam,
            Description: null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("REVIEW_ALREADY_REPORTED");
    }
}
