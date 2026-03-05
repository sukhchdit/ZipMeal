using FluentAssertions;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Reviews.Commands.ResolveReviewReport;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.Reviews;

public sealed class ResolveReviewReportCommandHandlerTests
{
    private static readonly Guid ReportId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

    private readonly IAppDbContext _db;
    private readonly ResolveReviewReportCommandHandler _handler;
    private readonly List<ReviewReport> _reports;
    private readonly List<Review> _reviews;
    private readonly Restaurant _restaurant;

    public ResolveReviewReportCommandHandlerTests()
    {
        _restaurant = new RestaurantBuilder().Build();
        var review = new ReviewBuilder()
            .WithRestaurant(_restaurant)
            .Build();

        _reviews = [review];

        var report = new ReviewReport
        {
            Id = ReportId,
            ReviewId = TestConstants.ReviewId,
            UserId = Guid.NewGuid(),
            Reason = ReviewReportReason.Spam,
            Status = ReviewReportStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
            Review = review,
        };

        _reports = [report];

        _db = MockDbContextFactory.Create(
            reviews: _reviews,
            reviewReports: _reports,
            restaurants: [_restaurant]);

        _handler = new ResolveReviewReportCommandHandler(_db);
    }

    [Fact]
    public async Task Handle_Dismissed_ResolvesReport()
    {
        var command = new ResolveReviewReportCommand(
            ReportId: ReportId,
            AdminId: TestConstants.AdminId,
            Status: ReviewReportStatus.Dismissed,
            AdminNotes: "Not a violation");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _reports[0].Status.Should().Be(ReviewReportStatus.Dismissed);
        _reports[0].AdminNotes.Should().Be("Not a violation");
        _reports[0].ResolvedByAdminId.Should().Be(TestConstants.AdminId);
        _reports[0].ResolvedAt.Should().NotBeNull();
        _reviews[0].IsVisible.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ActionTaken_HidesReview()
    {
        var command = new ResolveReviewReportCommand(
            ReportId: ReportId,
            AdminId: TestConstants.AdminId,
            Status: ReviewReportStatus.ActionTaken,
            AdminNotes: "Violates guidelines");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _reports[0].Status.Should().Be(ReviewReportStatus.ActionTaken);
        _reviews[0].IsVisible.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ReportNotFound_ReturnsFailure()
    {
        var command = new ResolveReviewReportCommand(
            ReportId: Guid.NewGuid(),
            AdminId: TestConstants.AdminId,
            Status: ReviewReportStatus.Dismissed,
            AdminNotes: null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("REPORT_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_AlreadyResolved_ReturnsFailure()
    {
        _reports[0].Status = ReviewReportStatus.ActionTaken;

        var command = new ResolveReviewReportCommand(
            ReportId: ReportId,
            AdminId: TestConstants.AdminId,
            Status: ReviewReportStatus.Dismissed,
            AdminNotes: null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("REPORT_ALREADY_RESOLVED");
    }
}
