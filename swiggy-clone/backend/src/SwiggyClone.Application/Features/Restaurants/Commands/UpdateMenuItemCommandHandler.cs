using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Application.Features.Discovery.Notifications;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

internal sealed class UpdateMenuItemCommandHandler(IAppDbContext db, IPublisher publisher)
    : IRequestHandler<UpdateMenuItemCommand, Result<MenuItemDto>>
{
    public async Task<Result<MenuItemDto>> Handle(
        UpdateMenuItemCommand request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (ownershipResult.IsFailure)
            return Result<MenuItemDto>.Failure(ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var menuItem = await db.MenuItems
            .Include(mi => mi.Variants)
            .Include(mi => mi.Addons)
            .FirstOrDefaultAsync(mi => mi.Id == request.MenuItemId && mi.RestaurantId == request.RestaurantId && !mi.IsDeleted, ct);

        if (menuItem is null)
            return Result<MenuItemDto>.Failure("MENU_ITEM_NOT_FOUND", "Menu item not found.");

        menuItem.CategoryId = request.CategoryId;
        menuItem.Name = request.Name;
        menuItem.Description = request.Description;
        menuItem.Price = request.Price;
        menuItem.DiscountedPrice = request.DiscountedPrice;
        menuItem.ImageUrl = request.ImageUrl;
        menuItem.IsVeg = request.IsVeg;
        menuItem.IsAvailable = request.IsAvailable;
        menuItem.IsBestseller = request.IsBestseller;
        menuItem.PreparationTimeMin = request.PreparationTimeMin;
        menuItem.SortOrder = request.SortOrder;
        menuItem.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);
        await publisher.Publish(new MenuItemIndexRequested(menuItem.Id), ct);

        var variantDtos = menuItem.Variants
            .Select(v => new MenuItemVariantDto(
                v.Id, v.Name, v.PriceAdjustment, v.IsDefault, v.IsAvailable, v.SortOrder))
            .ToList();

        var addonDtos = menuItem.Addons
            .Select(a => new MenuItemAddonDto(
                a.Id, a.Name, a.Price, a.IsVeg, a.IsAvailable, a.MaxQuantity, a.SortOrder))
            .ToList();

        var dto = new MenuItemDto(
            menuItem.Id, menuItem.CategoryId, menuItem.Name, menuItem.Description,
            menuItem.Price, menuItem.DiscountedPrice, menuItem.ImageUrl,
            menuItem.IsVeg, menuItem.IsAvailable, menuItem.IsBestseller,
            menuItem.PreparationTimeMin, menuItem.SortOrder,
            variantDtos, addonDtos);

        return Result<MenuItemDto>.Success(dto);
    }
}
