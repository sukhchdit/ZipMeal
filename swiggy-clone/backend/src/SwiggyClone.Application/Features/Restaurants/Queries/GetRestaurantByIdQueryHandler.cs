using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Queries;

internal sealed class GetRestaurantByIdQueryHandler(IAppDbContext db)
    : IRequestHandler<GetRestaurantByIdQuery, Result<RestaurantDto>>
{
    public async Task<Result<RestaurantDto>> Handle(
        GetRestaurantByIdQuery request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (!ownershipResult.IsSuccess)
            return Result<RestaurantDto>.Failure(ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var restaurant = ownershipResult.Value!;

        var cuisines = await db.RestaurantCuisines
            .AsNoTracking()
            .Where(rc => rc.RestaurantId == request.RestaurantId)
            .Include(rc => rc.CuisineType)
            .Select(rc => new CuisineTypeDto(
                rc.CuisineType.Id,
                rc.CuisineType.Name,
                rc.CuisineType.IconUrl))
            .ToListAsync(ct);

        var dto = new RestaurantDto(
            restaurant.Id,
            restaurant.Name,
            restaurant.Slug,
            restaurant.Description,
            restaurant.PhoneNumber,
            restaurant.Email,
            restaurant.LogoUrl,
            restaurant.BannerUrl,
            restaurant.AddressLine1,
            restaurant.AddressLine2,
            restaurant.City,
            restaurant.State,
            restaurant.PostalCode,
            restaurant.Latitude,
            restaurant.Longitude,
            restaurant.AverageRating,
            restaurant.TotalRatings,
            restaurant.AvgDeliveryTimeMin,
            restaurant.AvgCostForTwo,
            restaurant.IsVegOnly,
            restaurant.IsAcceptingOrders,
            restaurant.IsDineInEnabled,
            restaurant.FssaiLicense,
            restaurant.GstNumber,
            restaurant.Status.ToString(),
            restaurant.CreatedAt,
            cuisines);

        return Result<RestaurantDto>.Success(dto);
    }
}
