using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Common.Helpers;

public static class RestaurantOwnershipHelper
{
    public static async Task<Result<Restaurant>> VerifyOwnership(
        IAppDbContext db, Guid restaurantId, Guid ownerId, CancellationToken ct)
    {
        var restaurant = await db.Restaurants
            .FirstOrDefaultAsync(r => r.Id == restaurantId, ct);

        if (restaurant is null)
            return Result<Restaurant>.Failure("RESTAURANT_NOT_FOUND", "Restaurant not found.");

        if (restaurant.OwnerId != ownerId)
            return Result<Restaurant>.Failure("FORBIDDEN", "You do not own this restaurant.");

        return Result<Restaurant>.Success(restaurant);
    }
}
