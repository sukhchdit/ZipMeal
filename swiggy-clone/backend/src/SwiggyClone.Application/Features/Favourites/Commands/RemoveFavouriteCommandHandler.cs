using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Favourites.Commands;

internal sealed class RemoveFavouriteCommandHandler(IAppDbContext db)
    : IRequestHandler<RemoveFavouriteCommand, Result>
{
    public async Task<Result> Handle(RemoveFavouriteCommand request, CancellationToken ct)
    {
        var favourite = await db.UserFavorites
            .FirstOrDefaultAsync(f => f.UserId == request.UserId && f.RestaurantId == request.RestaurantId, ct);

        if (favourite is null)
            return Result.Success(); // Idempotent

        db.UserFavorites.Remove(favourite);
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
