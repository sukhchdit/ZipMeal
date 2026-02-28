using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

internal sealed class UpdateMenuItemAddonCommandHandler(IAppDbContext db)
    : IRequestHandler<UpdateMenuItemAddonCommand, Result<MenuItemAddonDto>>
{
    public async Task<Result<MenuItemAddonDto>> Handle(
        UpdateMenuItemAddonCommand request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (ownershipResult.IsFailure)
            return Result<MenuItemAddonDto>.Failure(ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var addon = await db.MenuItemAddons
            .FirstOrDefaultAsync(a => a.Id == request.AddonId && a.MenuItemId == request.MenuItemId, ct);

        if (addon is null)
            return Result<MenuItemAddonDto>.Failure("ADDON_NOT_FOUND", "Menu item addon not found.");

        addon.Name = request.Name;
        addon.Price = request.Price;
        addon.IsVeg = request.IsVeg;
        addon.IsAvailable = request.IsAvailable;
        addon.MaxQuantity = request.MaxQuantity;
        addon.SortOrder = request.SortOrder;

        await db.SaveChangesAsync(ct);

        var dto = new MenuItemAddonDto(
            addon.Id, addon.Name, addon.Price,
            addon.IsVeg, addon.IsAvailable, addon.MaxQuantity, addon.SortOrder);

        return Result<MenuItemAddonDto>.Success(dto);
    }
}
