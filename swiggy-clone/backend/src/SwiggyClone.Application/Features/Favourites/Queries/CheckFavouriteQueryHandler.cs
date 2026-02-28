using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Favourites.Queries;

internal sealed class CheckFavouriteQueryHandler(IAppDbContext db)
    : IRequestHandler<CheckFavouriteQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(CheckFavouriteQuery request, CancellationToken ct)
    {
        var isFavourited = await db.UserFavorites
            .AsNoTracking()
            .AnyAsync(f => f.UserId == request.UserId && f.RestaurantId == request.RestaurantId, ct);

        return Result<bool>.Success(isFavourited);
    }
}
