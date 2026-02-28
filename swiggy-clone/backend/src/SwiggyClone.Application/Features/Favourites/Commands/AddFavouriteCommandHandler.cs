using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Favourites.Commands;

internal sealed class AddFavouriteCommandHandler(IAppDbContext db)
    : IRequestHandler<AddFavouriteCommand, Result>
{
    public async Task<Result> Handle(AddFavouriteCommand request, CancellationToken ct)
    {
        // Verify restaurant exists and is approved
        var restaurantExists = await db.Restaurants
            .AsNoTracking()
            .AnyAsync(r => r.Id == request.RestaurantId && r.Status == RestaurantStatus.Approved, ct);

        if (!restaurantExists)
            return Result.Failure("RESTAURANT_NOT_FOUND", "Restaurant not found or not approved.");

        // Check if already favourited
        var alreadyFavourited = await db.UserFavorites
            .AsNoTracking()
            .AnyAsync(f => f.UserId == request.UserId && f.RestaurantId == request.RestaurantId, ct);

        if (alreadyFavourited)
            return Result.Success(); // Idempotent

        db.UserFavorites.Add(new UserFavorite
        {
            UserId = request.UserId,
            RestaurantId = request.RestaurantId,
            CreatedAt = DateTimeOffset.UtcNow,
        });

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
