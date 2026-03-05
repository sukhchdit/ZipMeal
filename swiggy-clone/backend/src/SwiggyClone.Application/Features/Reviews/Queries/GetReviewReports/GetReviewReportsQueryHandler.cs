using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Reviews.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Queries.GetReviewReports;

internal sealed class GetReviewReportsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetReviewReportsQuery, Result<PagedResult<ReviewReportDto>>>
{
    public async Task<Result<PagedResult<ReviewReportDto>>> Handle(GetReviewReportsQuery request, CancellationToken ct)
    {
        var query = db.ReviewReports
            .AsNoTracking()
            .Include(r => r.Review)
                .ThenInclude(r => r.User)
            .Include(r => r.User)
            .AsQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(r => r.Status == request.Status.Value);
        }

        query = query.OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync(ct);

        var reports = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var items = reports.Select(r => new ReviewReportDto(
            r.Id,
            r.ReviewId,
            r.Review.ReviewText,
            r.Review.Rating,
            r.Review.User.FullName,
            r.User.FullName,
            r.Reason,
            r.Description,
            r.Status,
            r.AdminNotes,
            r.CreatedAt,
            r.ResolvedAt)).ToList();

        var result = new PagedResult<ReviewReportDto>(items, totalCount, request.Page, request.PageSize);
        return Result<PagedResult<ReviewReportDto>>.Success(result);
    }
}
