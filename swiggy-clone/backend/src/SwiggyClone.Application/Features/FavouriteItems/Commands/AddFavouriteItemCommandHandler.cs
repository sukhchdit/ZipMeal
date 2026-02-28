using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.FavouriteItems.Commands;

internal sealed class AddFavouriteItemCommandHandler(IAppDbContext db)
    : IRequestHandler<AddFavouriteItemCommand, Result>
{
    public async Task<Result> Handle(AddFavouriteItemCommand request, CancellationToken ct)
    {
        var menuItemExists = await db.MenuItems
            .AsNoTracking()
            .AnyAsync(m => m.Id == request.MenuItemId, ct);

        if (!menuItemExists)
            return Result.Failure("MENU_ITEM_NOT_FOUND", "Menu item not found.");

        var alreadyFavourited = await db.UserFavoriteItems
            .AsNoTracking()
            .AnyAsync(f => f.UserId == request.UserId && f.MenuItemId == request.MenuItemId, ct);

        if (alreadyFavourited)
            return Result.Success(); // Idempotent

        db.UserFavoriteItems.Add(new UserFavoriteItem
        {
            UserId = request.UserId,
            MenuItemId = request.MenuItemId,
            CreatedAt = DateTimeOffset.UtcNow,
        });

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
