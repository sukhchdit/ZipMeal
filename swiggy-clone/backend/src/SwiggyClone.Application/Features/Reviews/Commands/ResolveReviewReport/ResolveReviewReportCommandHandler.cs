using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Commands.ResolveReviewReport;

internal sealed class ResolveReviewReportCommandHandler(IAppDbContext db)
    : IRequestHandler<ResolveReviewReportCommand, Result>
{
    public async Task<Result> Handle(ResolveReviewReportCommand request, CancellationToken ct)
    {
        var report = await db.ReviewReports
            .Include(r => r.Review)
                .ThenInclude(r => r.Restaurant)
            .FirstOrDefaultAsync(r => r.Id == request.ReportId, ct);

        if (report is null)
            return Result.Failure("REPORT_NOT_FOUND", "Report not found.");

        if (report.Status is not ReviewReportStatus.Pending and not ReviewReportStatus.Reviewed)
            return Result.Failure("REPORT_ALREADY_RESOLVED", "This report has already been resolved.");

        report.Status = request.Status;
        report.AdminNotes = request.AdminNotes;
        report.ResolvedByAdminId = request.AdminId;
        report.ResolvedAt = DateTimeOffset.UtcNow;

        if (request.Status == ReviewReportStatus.ActionTaken)
        {
            report.Review.IsVisible = false;
            report.Review.UpdatedAt = DateTimeOffset.UtcNow;

            var restaurant = report.Review.Restaurant;
            var visibleReviews = await db.Reviews
                .Where(r => r.RestaurantId == restaurant.Id && r.IsVisible && r.Id != report.ReviewId)
                .ToListAsync(ct);

            restaurant.TotalRatings = visibleReviews.Count;
            restaurant.AverageRating = visibleReviews.Count > 0
                ? Math.Round((decimal)visibleReviews.Average(r => r.Rating), 1)
                : 0;
        }

        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
