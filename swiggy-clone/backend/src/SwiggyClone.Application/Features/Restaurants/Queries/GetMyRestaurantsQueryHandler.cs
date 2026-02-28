using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Queries;

internal sealed class GetMyRestaurantsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetMyRestaurantsQuery, Result<List<RestaurantSummaryDto>>>
{
    public async Task<Result<List<RestaurantSummaryDto>>> Handle(
        GetMyRestaurantsQuery request, CancellationToken ct)
    {
        var restaurants = await db.Restaurants
            .AsNoTracking()
            .Where(r => r.OwnerId == request.OwnerId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RestaurantSummaryDto(
                r.Id,
                r.Name,
                r.Slug,
                r.LogoUrl,
                r.City,
                r.AverageRating,
                r.TotalRatings,
                r.IsAcceptingOrders,
                r.Status.ToString()))
            .ToListAsync(ct);

        return Result<List<RestaurantSummaryDto>>.Success(restaurants);
    }
}
