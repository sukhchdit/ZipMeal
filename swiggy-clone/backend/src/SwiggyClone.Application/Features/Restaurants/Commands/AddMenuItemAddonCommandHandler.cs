using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

internal sealed class AddMenuItemAddonCommandHandler(IAppDbContext db)
    : IRequestHandler<AddMenuItemAddonCommand, Result<MenuItemAddonDto>>
{
    public async Task<Result<MenuItemAddonDto>> Handle(
        AddMenuItemAddonCommand request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (ownershipResult.IsFailure)
            return Result<MenuItemAddonDto>.Failure(ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var menuItemExists = await db.MenuItems
            .AnyAsync(mi => mi.Id == request.MenuItemId && mi.RestaurantId == request.RestaurantId && !mi.IsDeleted, ct);

        if (!menuItemExists)
            return Result<MenuItemAddonDto>.Failure("MENU_ITEM_NOT_FOUND", "Menu item not found.");

        var addon = new MenuItemAddon
        {
            Id = Guid.CreateVersion7(),
            MenuItemId = request.MenuItemId,
            Name = request.Name,
            Price = request.Price,
            IsVeg = request.IsVeg,
            IsAvailable = request.IsAvailable,
            MaxQuantity = request.MaxQuantity,
            SortOrder = request.SortOrder,
            CreatedAt = DateTimeOffset.UtcNow
        };

        db.MenuItemAddons.Add(addon);
        await db.SaveChangesAsync(ct);

        var dto = new MenuItemAddonDto(
            addon.Id, addon.Name, addon.Price,
            addon.IsVeg, addon.IsAvailable, addon.MaxQuantity, addon.SortOrder);

        return Result<MenuItemAddonDto>.Success(dto);
    }
}
