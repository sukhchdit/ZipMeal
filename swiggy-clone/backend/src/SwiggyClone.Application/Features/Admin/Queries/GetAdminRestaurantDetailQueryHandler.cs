using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Admin.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Admin.Queries;

internal sealed class GetAdminRestaurantDetailQueryHandler(IAppDbContext db)
    : IRequestHandler<GetAdminRestaurantDetailQuery, Result<AdminRestaurantDto>>
{
    public async Task<Result<AdminRestaurantDto>> Handle(
        GetAdminRestaurantDetailQuery request, CancellationToken ct)
    {
        var restaurant = await db.Restaurants
            .AsNoTracking()
            .Include(r => r.Owner)
            .FirstOrDefaultAsync(r => r.Id == request.RestaurantId, ct);

        if (restaurant is null)
            return Result<AdminRestaurantDto>.Failure("RESTAURANT_NOT_FOUND", "Restaurant not found.");

        return Result<AdminRestaurantDto>.Success(new AdminRestaurantDto(
            restaurant.Id,
            restaurant.Name,
            restaurant.Slug,
            restaurant.Description,
            restaurant.City,
            restaurant.State,
            restaurant.Owner.FullName,
            restaurant.Owner.PhoneNumber,
            restaurant.LogoUrl,
            restaurant.Status,
            restaurant.StatusReason,
            restaurant.AverageRating,
            restaurant.TotalRatings,
            restaurant.IsAcceptingOrders,
            restaurant.FssaiLicense,
            restaurant.GstNumber,
            restaurant.CreatedAt));
    }
}
