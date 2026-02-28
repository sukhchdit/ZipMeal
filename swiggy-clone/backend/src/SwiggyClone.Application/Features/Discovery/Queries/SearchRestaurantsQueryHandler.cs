using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Discovery.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Discovery.Queries;

internal sealed class SearchRestaurantsQueryHandler(
    ISearchService searchService,
    IAppDbContext db,
    ILogger<SearchRestaurantsQueryHandler> logger)
    : IRequestHandler<SearchRestaurantsQuery, Result<List<CustomerRestaurantDto>>>
{
    public async Task<Result<List<CustomerRestaurantDto>>> Handle(
        SearchRestaurantsQuery request, CancellationToken ct)
    {
        var term = request.Term.Trim();
        var pageSize = Math.Clamp(request.PageSize, 1, 50);

        // Try Elasticsearch first
        if (await searchService.IsAvailableAsync(ct))
        {
            try
            {
                var esResults = await searchService.SearchRestaurantsAsync(
                    term, request.City, pageSize, ct);

                var dtos = esResults.Select(r => new CustomerRestaurantDto(
                    r.Id, r.Name, r.Slug, r.LogoUrl, r.BannerUrl, r.City,
                    r.AverageRating, r.TotalRatings, r.AvgDeliveryTimeMin,
                    r.AvgCostForTwo, r.IsVegOnly, r.IsAcceptingOrders,
                    r.IsDineInEnabled, r.Cuisines)).ToList();

                return Result<List<CustomerRestaurantDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Elasticsearch search failed, falling back to EF Core");
            }
        }

        // Fallback: original EF LINQ approach
        return await SearchViaEfCoreAsync(term, request.City, pageSize, ct);
    }

    private async Task<Result<List<CustomerRestaurantDto>>> SearchViaEfCoreAsync(
        string term, string? city, int pageSize, CancellationToken ct)
    {
        var pattern = $"%{term}%";

        var query = db.Restaurants
            .AsNoTracking()
            .Where(r => r.Status == RestaurantStatus.Approved)
            .Where(r =>
                EF.Functions.Like(r.Name, pattern) ||
                (r.Description != null && EF.Functions.Like(r.Description, pattern)) ||
                r.RestaurantCuisines.Any(rc => EF.Functions.Like(rc.CuisineType.Name, pattern)));

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(r => r.City != null && EF.Functions.Like(r.City, city));

        var results = await query
            .OrderByDescending(r => r.AverageRating)
            .ThenByDescending(r => r.TotalRatings)
            .Take(pageSize)
            .Select(r => new CustomerRestaurantDto(
                r.Id, r.Name, r.Slug, r.LogoUrl, r.BannerUrl, r.City,
                r.AverageRating, r.TotalRatings, r.AvgDeliveryTimeMin,
                r.AvgCostForTwo, r.IsVegOnly, r.IsAcceptingOrders,
                r.IsDineInEnabled,
                r.RestaurantCuisines
                    .Select(rc => rc.CuisineType.Name)
                    .ToList()))
            .ToListAsync(ct);

        return Result<List<CustomerRestaurantDto>>.Success(results);
    }
}
