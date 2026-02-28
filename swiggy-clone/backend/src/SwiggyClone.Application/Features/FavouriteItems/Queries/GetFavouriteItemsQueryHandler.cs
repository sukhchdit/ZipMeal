using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.FavouriteItems.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.FavouriteItems.Queries;

internal sealed class GetFavouriteItemsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetFavouriteItemsQuery, Result<List<FavouriteItemDto>>>
{
    public async Task<Result<List<FavouriteItemDto>>> Handle(
        GetFavouriteItemsQuery request, CancellationToken ct)
    {
        var items = await db.UserFavoriteItems
            .AsNoTracking()
            .Where(f => f.UserId == request.UserId)
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => new FavouriteItemDto(
                f.MenuItem.Id,
                f.MenuItem.Name,
                f.MenuItem.ImageUrl,
                f.MenuItem.Price,
                f.MenuItem.DiscountedPrice,
                f.MenuItem.IsVeg,
                f.MenuItem.IsAvailable,
                f.MenuItem.RestaurantId,
                f.MenuItem.Restaurant.Name,
                f.CreatedAt))
            .ToListAsync(ct);

        return Result<List<FavouriteItemDto>>.Success(items);
    }
}
