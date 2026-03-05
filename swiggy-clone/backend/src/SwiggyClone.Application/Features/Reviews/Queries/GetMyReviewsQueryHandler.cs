using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Reviews.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Queries;

internal sealed class GetMyReviewsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetMyReviewsQuery, Result<PagedResult<ReviewDto>>>
{
    public async Task<Result<PagedResult<ReviewDto>>> Handle(GetMyReviewsQuery request, CancellationToken ct)
    {
        var query = db.Reviews
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.Photos)
            .Where(r => r.UserId == request.UserId)
            .OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync(ct);

        var reviews = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var items = reviews.Select(r => new ReviewDto(
            r.Id,
            r.OrderId,
            r.UserId,
            r.User.FullName,
            r.User.AvatarUrl,
            r.RestaurantId,
            r.Rating,
            r.ReviewText,
            r.DeliveryRating,
            r.IsAnonymous,
            r.IsVisible,
            r.RestaurantReply,
            r.RepliedAt,
            r.CreatedAt,
            r.Photos.OrderBy(p => p.SortOrder)
                .Select(p => new ReviewPhotoDto(p.Id, p.PhotoUrl, p.SortOrder))
                .ToList(),
            r.HelpfulCount,
            null,
            null
        )).ToList();

        var result = new PagedResult<ReviewDto>(items, totalCount, request.Page, request.PageSize);
        return Result<PagedResult<ReviewDto>>.Success(result);
    }
}
