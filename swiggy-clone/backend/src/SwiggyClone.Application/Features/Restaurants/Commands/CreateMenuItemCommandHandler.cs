using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Application.Features.Discovery.Notifications;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

internal sealed class CreateMenuItemCommandHandler(IAppDbContext db, IPublisher publisher)
    : IRequestHandler<CreateMenuItemCommand, Result<MenuItemDto>>
{
    public async Task<Result<MenuItemDto>> Handle(
        CreateMenuItemCommand request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (ownershipResult.IsFailure)
            return Result<MenuItemDto>.Failure(ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var categoryExists = await db.MenuCategories
            .AnyAsync(c => c.Id == request.CategoryId && c.RestaurantId == request.RestaurantId, ct);

        if (!categoryExists)
            return Result<MenuItemDto>.Failure("CATEGORY_NOT_FOUND", "Menu category not found for this restaurant.");

        var now = DateTimeOffset.UtcNow;

        var menuItem = new MenuItem
        {
            Id = Guid.CreateVersion7(),
            CategoryId = request.CategoryId,
            RestaurantId = request.RestaurantId,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            DiscountedPrice = request.DiscountedPrice,
            ImageUrl = request.ImageUrl,
            IsVeg = request.IsVeg,
            IsAvailable = request.IsAvailable,
            IsBestseller = request.IsBestseller,
            PreparationTimeMin = request.PreparationTimeMin,
            SortOrder = request.SortOrder,
            CreatedAt = now,
            UpdatedAt = now
        };

        db.MenuItems.Add(menuItem);

        var variantDtos = new List<MenuItemVariantDto>();
        if (request.Variants is { Count: > 0 })
        {
            foreach (var v in request.Variants)
            {
                var variant = new MenuItemVariant
                {
                    Id = Guid.CreateVersion7(),
                    MenuItemId = menuItem.Id,
                    Name = v.Name,
                    PriceAdjustment = v.PriceAdjustment,
                    IsDefault = v.IsDefault,
                    IsAvailable = v.IsAvailable,
                    SortOrder = v.SortOrder,
                    CreatedAt = now
                };

                db.MenuItemVariants.Add(variant);
                variantDtos.Add(new MenuItemVariantDto(
                    variant.Id, variant.Name, variant.PriceAdjustment,
                    variant.IsDefault, variant.IsAvailable, variant.SortOrder));
            }
        }

        var addonDtos = new List<MenuItemAddonDto>();
        if (request.Addons is { Count: > 0 })
        {
            foreach (var a in request.Addons)
            {
                var addon = new MenuItemAddon
                {
                    Id = Guid.CreateVersion7(),
                    MenuItemId = menuItem.Id,
                    Name = a.Name,
                    Price = a.Price,
                    IsVeg = a.IsVeg,
                    IsAvailable = a.IsAvailable,
                    MaxQuantity = a.MaxQuantity,
                    SortOrder = a.SortOrder,
                    CreatedAt = now
                };

                db.MenuItemAddons.Add(addon);
                addonDtos.Add(new MenuItemAddonDto(
                    addon.Id, addon.Name, addon.Price,
                    addon.IsVeg, addon.IsAvailable, addon.MaxQuantity, addon.SortOrder));
            }
        }

        await db.SaveChangesAsync(ct);
        await publisher.Publish(new MenuItemIndexRequested(menuItem.Id), ct);

        var dto = new MenuItemDto(
            menuItem.Id, menuItem.CategoryId, menuItem.Name, menuItem.Description,
            menuItem.Price, menuItem.DiscountedPrice, menuItem.ImageUrl,
            menuItem.IsVeg, menuItem.IsAvailable, menuItem.IsBestseller,
            menuItem.PreparationTimeMin, menuItem.SortOrder,
            variantDtos, addonDtos);

        return Result<MenuItemDto>.Success(dto);
    }
}
