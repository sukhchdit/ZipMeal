using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Discovery.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Discovery.Queries;

internal sealed class BrowseRestaurantsQueryHandler(IAppDbContext db)
    : IRequestHandler<BrowseRestaurantsQuery, Result<CursorPagedResult<CustomerRestaurantDto>>>
{
    public async Task<Result<CursorPagedResult<CustomerRestaurantDto>>> Handle(
        BrowseRestaurantsQuery request, CancellationToken ct)
    {
        var query = db.Restaurants
            .AsNoTracking()
            .Where(r => r.Status == RestaurantStatus.Approved && r.IsAcceptingOrders);

        // ── Filters ──
        if (!string.IsNullOrWhiteSpace(request.City))
            query = query.Where(r => r.City != null && EF.Functions.Like(r.City, request.City));

        if (request.CuisineId.HasValue)
            query = query.Where(r => r.RestaurantCuisines.Any(rc => rc.CuisineId == request.CuisineId.Value));

        if (request.IsVegOnly == true)
            query = query.Where(r => r.IsVegOnly);

        if (request.MinRating.HasValue)
            query = query.Where(r => r.AverageRating >= request.MinRating.Value);

        if (request.MaxCostForTwo.HasValue)
            query = query.Where(r => r.AvgCostForTwo != null && r.AvgCostForTwo <= request.MaxCostForTwo.Value);

        // ── Sorting ──
        query = request.SortBy?.ToLowerInvariant() switch
        {
            "rating" => query.OrderByDescending(r => r.AverageRating).ThenBy(r => r.Id),
            "deliverytime" => query.OrderBy(r => r.AvgDeliveryTimeMin ?? int.MaxValue).ThenBy(r => r.Id),
            "costlowtohigh" => query.OrderBy(r => r.AvgCostForTwo ?? int.MaxValue).ThenBy(r => r.Id),
            "costhightolow" => query.OrderByDescending(r => r.AvgCostForTwo ?? 0).ThenBy(r => r.Id),
            _ => query.OrderByDescending(r => r.AverageRating).ThenBy(r => r.Id),
        };

        // ── Cursor pagination ──
        if (!string.IsNullOrEmpty(request.Cursor) && Guid.TryParse(request.Cursor, out var cursorId))
        {
            query = query.Where(r => r.Id.CompareTo(cursorId) > 0);
        }

        var pageSize = Math.Clamp(request.PageSize, 1, 50);

        var restaurants = await query
            .Take(pageSize + 1)
            .Select(r => new CustomerRestaurantDto(
                r.Id,
                r.Name,
                r.Slug,
                r.LogoUrl,
                r.BannerUrl,
                r.City,
                r.AverageRating,
                r.TotalRatings,
                r.AvgDeliveryTimeMin,
                r.AvgCostForTwo,
                r.IsVegOnly,
                r.IsAcceptingOrders,
                r.IsDineInEnabled,
                r.RestaurantCuisines
                    .Select(rc => rc.CuisineType.Name)
                    .ToList()))
            .ToListAsync(ct);

        var hasMore = restaurants.Count > pageSize;
        if (hasMore) restaurants.RemoveAt(restaurants.Count - 1);

        var nextCursor = hasMore && restaurants.Count > 0
            ? restaurants[^1].Id.ToString()
            : null;

        return Result<CursorPagedResult<CustomerRestaurantDto>>.Success(
            new CursorPagedResult<CustomerRestaurantDto>(
                restaurants, nextCursor, request.Cursor, hasMore, pageSize));
    }
}
