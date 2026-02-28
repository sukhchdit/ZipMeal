using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

internal sealed class AddMenuItemVariantCommandHandler(IAppDbContext db)
    : IRequestHandler<AddMenuItemVariantCommand, Result<MenuItemVariantDto>>
{
    public async Task<Result<MenuItemVariantDto>> Handle(
        AddMenuItemVariantCommand request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (ownershipResult.IsFailure)
            return Result<MenuItemVariantDto>.Failure(ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var menuItemExists = await db.MenuItems
            .AnyAsync(mi => mi.Id == request.MenuItemId && mi.RestaurantId == request.RestaurantId && !mi.IsDeleted, ct);

        if (!menuItemExists)
            return Result<MenuItemVariantDto>.Failure("MENU_ITEM_NOT_FOUND", "Menu item not found.");

        var variant = new MenuItemVariant
        {
            Id = Guid.CreateVersion7(),
            MenuItemId = request.MenuItemId,
            Name = request.Name,
            PriceAdjustment = request.PriceAdjustment,
            IsDefault = request.IsDefault,
            IsAvailable = request.IsAvailable,
            SortOrder = request.SortOrder,
            CreatedAt = DateTimeOffset.UtcNow
        };

        db.MenuItemVariants.Add(variant);
        await db.SaveChangesAsync(ct);

        var dto = new MenuItemVariantDto(
            variant.Id, variant.Name, variant.PriceAdjustment,
            variant.IsDefault, variant.IsAvailable, variant.SortOrder);

        return Result<MenuItemVariantDto>.Success(dto);
    }
}
