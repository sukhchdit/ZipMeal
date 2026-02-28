using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

internal sealed class UpdateMenuItemVariantCommandHandler(IAppDbContext db)
    : IRequestHandler<UpdateMenuItemVariantCommand, Result<MenuItemVariantDto>>
{
    public async Task<Result<MenuItemVariantDto>> Handle(
        UpdateMenuItemVariantCommand request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (ownershipResult.IsFailure)
            return Result<MenuItemVariantDto>.Failure(ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var variant = await db.MenuItemVariants
            .FirstOrDefaultAsync(v => v.Id == request.VariantId && v.MenuItemId == request.MenuItemId, ct);

        if (variant is null)
            return Result<MenuItemVariantDto>.Failure("VARIANT_NOT_FOUND", "Menu item variant not found.");

        variant.Name = request.Name;
        variant.PriceAdjustment = request.PriceAdjustment;
        variant.IsDefault = request.IsDefault;
        variant.IsAvailable = request.IsAvailable;
        variant.SortOrder = request.SortOrder;

        await db.SaveChangesAsync(ct);

        var dto = new MenuItemVariantDto(
            variant.Id, variant.Name, variant.PriceAdjustment,
            variant.IsDefault, variant.IsAvailable, variant.SortOrder);

        return Result<MenuItemVariantDto>.Success(dto);
    }
}
