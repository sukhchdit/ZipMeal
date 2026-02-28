using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Discovery.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Favourites.Queries;

internal sealed class GetFavouritesQueryHandler(IAppDbContext db)
    : IRequestHandler<GetFavouritesQuery, Result<List<CustomerRestaurantDto>>>
{
    public async Task<Result<List<CustomerRestaurantDto>>> Handle(
        GetFavouritesQuery request, CancellationToken ct)
    {
        var favourites = await db.UserFavorites
            .AsNoTracking()
            .Where(f => f.UserId == request.UserId)
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => new CustomerRestaurantDto(
                f.Restaurant.Id,
                f.Restaurant.Name,
                f.Restaurant.Slug,
                f.Restaurant.LogoUrl,
                f.Restaurant.BannerUrl,
                f.Restaurant.City,
                f.Restaurant.AverageRating,
                f.Restaurant.TotalRatings,
                f.Restaurant.AvgDeliveryTimeMin,
                f.Restaurant.AvgCostForTwo,
                f.Restaurant.IsVegOnly,
                f.Restaurant.IsAcceptingOrders,
                f.Restaurant.IsDineInEnabled,
                f.Restaurant.RestaurantCuisines
                    .Select(rc => rc.CuisineType.Name)
                    .ToList()))
            .ToListAsync(ct);

        return Result<List<CustomerRestaurantDto>>.Success(favourites);
    }
}
