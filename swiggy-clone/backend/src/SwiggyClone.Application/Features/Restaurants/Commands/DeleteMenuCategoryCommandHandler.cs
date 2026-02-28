using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

internal sealed class DeleteMenuCategoryCommandHandler(IAppDbContext db)
    : IRequestHandler<DeleteMenuCategoryCommand, Result>
{
    public async Task<Result> Handle(
        DeleteMenuCategoryCommand request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (ownershipResult.IsFailure)
            return Result.Failure(ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var category = await db.MenuCategories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.RestaurantId == request.RestaurantId, ct);

        if (category is null)
            return Result.Failure("CATEGORY_NOT_FOUND", "Menu category not found.");

        var hasItems = await db.MenuItems
            .AnyAsync(mi => mi.CategoryId == category.Id && !mi.IsDeleted, ct);

        if (hasItems)
            return Result.Failure("CATEGORY_HAS_ITEMS", "Cannot delete a category that contains menu items.");

        db.MenuCategories.Remove(category);
        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
