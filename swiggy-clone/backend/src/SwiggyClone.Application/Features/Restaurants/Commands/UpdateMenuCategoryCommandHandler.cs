using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

internal sealed class UpdateMenuCategoryCommandHandler(IAppDbContext db)
    : IRequestHandler<UpdateMenuCategoryCommand, Result<MenuCategoryDto>>
{
    public async Task<Result<MenuCategoryDto>> Handle(
        UpdateMenuCategoryCommand request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (ownershipResult.IsFailure)
            return Result<MenuCategoryDto>.Failure(ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var category = await db.MenuCategories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.RestaurantId == request.RestaurantId, ct);

        if (category is null)
            return Result<MenuCategoryDto>.Failure("CATEGORY_NOT_FOUND", "Menu category not found.");

        category.Name = request.Name;
        category.Description = request.Description;
        category.SortOrder = request.SortOrder;
        category.IsActive = request.IsActive;
        category.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);

        var itemCount = await db.MenuItems
            .CountAsync(mi => mi.CategoryId == category.Id && !mi.IsDeleted, ct);

        var dto = new MenuCategoryDto(
            category.Id, category.Name, category.Description,
            category.SortOrder, category.IsActive, itemCount);

        return Result<MenuCategoryDto>.Success(dto);
    }
}
