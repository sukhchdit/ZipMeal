using MediatR;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

internal sealed class CreateMenuCategoryCommandHandler(IAppDbContext db)
    : IRequestHandler<CreateMenuCategoryCommand, Result<MenuCategoryDto>>
{
    public async Task<Result<MenuCategoryDto>> Handle(
        CreateMenuCategoryCommand request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (ownershipResult.IsFailure)
            return Result<MenuCategoryDto>.Failure(ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var category = new MenuCategory
        {
            Id = Guid.CreateVersion7(),
            RestaurantId = request.RestaurantId,
            Name = request.Name,
            Description = request.Description,
            SortOrder = request.SortOrder,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        db.MenuCategories.Add(category);
        await db.SaveChangesAsync(ct);

        var dto = new MenuCategoryDto(
            category.Id, category.Name, category.Description,
            category.SortOrder, category.IsActive, 0);

        return Result<MenuCategoryDto>.Success(dto);
    }
}
