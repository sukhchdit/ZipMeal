using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Commands.ReportReview;

internal sealed class ReportReviewCommandHandler(IAppDbContext db)
    : IRequestHandler<ReportReviewCommand, Result>
{
    public async Task<Result> Handle(ReportReviewCommand request, CancellationToken ct)
    {
        var review = await db.Reviews
            .FirstOrDefaultAsync(r => r.Id == request.ReviewId && r.IsVisible, ct);

        if (review is null)
            return Result.Failure("REVIEW_NOT_FOUND", "Review not found.");

        if (review.UserId == request.UserId)
            return Result.Failure("CANNOT_REPORT_OWN_REVIEW", "You cannot report your own review.");

        var alreadyReported = await db.ReviewReports
            .AnyAsync(r => r.ReviewId == request.ReviewId && r.UserId == request.UserId, ct);

        if (alreadyReported)
            return Result.Failure("REVIEW_ALREADY_REPORTED", "You have already reported this review.");

        db.ReviewReports.Add(new ReviewReport
        {
            Id = Guid.CreateVersion7(),
            ReviewId = request.ReviewId,
            UserId = request.UserId,
            Reason = request.Reason,
            Description = request.Description,
            Status = ReviewReportStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
        });

        review.ReportCount++;

        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
