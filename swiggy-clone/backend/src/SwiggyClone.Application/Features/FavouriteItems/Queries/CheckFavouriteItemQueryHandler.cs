using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.FavouriteItems.Queries;

internal sealed class CheckFavouriteItemQueryHandler(IAppDbContext db)
    : IRequestHandler<CheckFavouriteItemQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(CheckFavouriteItemQuery request, CancellationToken ct)
    {
        var isFavourited = await db.UserFavoriteItems
            .AsNoTracking()
            .AnyAsync(f => f.UserId == request.UserId && f.MenuItemId == request.MenuItemId, ct);

        return Result<bool>.Success(isFavourited);
    }
}
