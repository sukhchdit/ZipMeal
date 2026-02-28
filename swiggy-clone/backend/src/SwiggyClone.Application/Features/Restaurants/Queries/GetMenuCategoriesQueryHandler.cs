using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Queries;

internal sealed class GetMenuCategoriesQueryHandler(IAppDbContext db)
    : IRequestHandler<GetMenuCategoriesQuery, Result<List<MenuCategoryDto>>>
{
    public async Task<Result<List<MenuCategoryDto>>> Handle(
        GetMenuCategoriesQuery request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (!ownershipResult.IsSuccess)
            return Result<List<MenuCategoryDto>>.Failure(ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var categories = await db.MenuCategories
            .AsNoTracking()
            .Where(c => c.RestaurantId == request.RestaurantId)
            .OrderBy(c => c.SortOrder)
            .Select(c => new MenuCategoryDto(
                c.Id,
                c.Name,
                c.Description,
                c.SortOrder,
                c.IsActive,
                c.MenuItems.Count(mi => !mi.IsDeleted)))
            .ToListAsync(ct);

        return Result<List<MenuCategoryDto>>.Success(categories);
    }
}
