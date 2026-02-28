using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Queries;

internal sealed class GetMenuItemByIdQueryHandler(IAppDbContext db)
    : IRequestHandler<GetMenuItemByIdQuery, Result<MenuItemDto>>
{
    public async Task<Result<MenuItemDto>> Handle(
        GetMenuItemByIdQuery request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (!ownershipResult.IsSuccess)
            return Result<MenuItemDto>.Failure(ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var item = await db.MenuItems
            .AsNoTracking()
            .Where(mi => mi.Id == request.MenuItemId && mi.RestaurantId == request.RestaurantId)
            .Select(mi => new MenuItemDto(
                mi.Id,
                mi.CategoryId,
                mi.Name,
                mi.Description,
                mi.Price,
                mi.DiscountedPrice,
                mi.ImageUrl,
                mi.IsVeg,
                mi.IsAvailable,
                mi.IsBestseller,
                mi.PreparationTimeMin,
                mi.SortOrder,
                mi.Variants
                    .OrderBy(v => v.SortOrder)
                    .Select(v => new MenuItemVariantDto(
                        v.Id, v.Name, v.PriceAdjustment, v.IsDefault, v.IsAvailable, v.SortOrder))
                    .ToList(),
                mi.Addons
                    .OrderBy(a => a.SortOrder)
                    .Select(a => new MenuItemAddonDto(
                        a.Id, a.Name, a.Price, a.IsVeg, a.IsAvailable, a.MaxQuantity, a.SortOrder))
                    .ToList()))
            .FirstOrDefaultAsync(ct);

        if (item is null)
            return Result<MenuItemDto>.Failure("MENU_ITEM_NOT_FOUND", "Menu item not found.");

        return Result<MenuItemDto>.Success(item);
    }
}
