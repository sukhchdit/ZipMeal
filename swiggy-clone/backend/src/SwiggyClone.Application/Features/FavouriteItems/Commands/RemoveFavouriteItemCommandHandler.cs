using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.FavouriteItems.Commands;

internal sealed class RemoveFavouriteItemCommandHandler(IAppDbContext db)
    : IRequestHandler<RemoveFavouriteItemCommand, Result>
{
    public async Task<Result> Handle(RemoveFavouriteItemCommand request, CancellationToken ct)
    {
        var favourite = await db.UserFavoriteItems
            .FirstOrDefaultAsync(f => f.UserId == request.UserId && f.MenuItemId == request.MenuItemId, ct);

        if (favourite is null)
            return Result.Success(); // Idempotent

        db.UserFavoriteItems.Remove(favourite);
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
