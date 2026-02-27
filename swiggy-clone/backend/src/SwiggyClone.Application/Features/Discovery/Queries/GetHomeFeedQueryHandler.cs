using System.Linq.Expressions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Discovery.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Discovery.Queries;

internal sealed class GetHomeFeedQueryHandler(IAppDbContext db)
    : IRequestHandler<GetHomeFeedQuery, Result<HomeFeedDto>>
{
    private static readonly Expression<Func<Restaurant, CustomerRestaurantDto>> Projection = r =>
        new CustomerRestaurantDto(
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
                .ToList());

    public async Task<Result<HomeFeedDto>> Handle(
        GetHomeFeedQuery request, CancellationToken ct)
    {
        // ── Cuisine chips ──
        var cuisines = await db.CuisineTypes
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .Select(c => new CuisineChipDto(c.Id, c.Name, c.IconUrl))
            .ToListAsync(ct);

        // ── Base query for approved restaurants ──
        var baseQuery = db.Restaurants
            .AsNoTracking()
            .Where(r => r.Status == RestaurantStatus.Approved);

        if (!string.IsNullOrWhiteSpace(request.City))
            baseQuery = baseQuery.Where(r => r.City != null && EF.Functions.Like(r.City, request.City));

        // ── Top Rated ──
        var topRated = await baseQuery
            .OrderByDescending(r => r.AverageRating)
            .ThenByDescending(r => r.TotalRatings)
            .Take(10)
            .Select(Projection)
            .ToListAsync(ct);

        // ── Popular (most ratings) ──
        var popular = await baseQuery
            .OrderByDescending(r => r.TotalRatings)
            .Take(10)
            .Select(Projection)
            .ToListAsync(ct);

        // ── Quick Delivery ──
        var quickDelivery = await baseQuery
            .Where(r => r.AvgDeliveryTimeMin != null && r.IsAcceptingOrders)
            .OrderBy(r => r.AvgDeliveryTimeMin)
            .Take(10)
            .Select(Projection)
            .ToListAsync(ct);

        // ── Banners (from database) ──
        var now = DateTimeOffset.UtcNow;
        var banners = await db.Banners
            .AsNoTracking()
            .Where(b => b.IsActive && b.ValidFrom <= now && b.ValidUntil >= now)
            .OrderBy(b => b.DisplayOrder)
            .Select(b => new BannerDto(b.Id.ToString(), b.ImageUrl, b.DeepLink))
            .ToListAsync(ct);

        var sections = new List<RestaurantSectionDto>();
        if (topRated.Count > 0) sections.Add(new("Top Rated", topRated));
        if (popular.Count > 0) sections.Add(new("Popular Near You", popular));
        if (quickDelivery.Count > 0) sections.Add(new("Quick Delivery", quickDelivery));

        return Result<HomeFeedDto>.Success(new HomeFeedDto(banners, cuisines, sections));
    }
}
