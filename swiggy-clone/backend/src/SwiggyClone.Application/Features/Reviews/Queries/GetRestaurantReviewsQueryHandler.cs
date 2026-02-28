using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Reviews.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Queries;

internal sealed class GetRestaurantReviewsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetRestaurantReviewsQuery, Result<PagedResult<ReviewDto>>>
{
    public async Task<Result<PagedResult<ReviewDto>>> Handle(GetRestaurantReviewsQuery request, CancellationToken ct)
    {
        var query = db.Reviews
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.Photos)
            .Where(r => r.RestaurantId == request.RestaurantId && r.IsVisible)
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
            r.IsAnonymous ? "Anonymous" : r.User.FullName,
            r.IsAnonymous ? null : r.User.AvatarUrl,
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
                .ToList()
        )).ToList();

        var result = new PagedResult<ReviewDto>(items, totalCount, request.Page, request.PageSize);
        return Result<PagedResult<ReviewDto>>.Success(result);
    }
}
