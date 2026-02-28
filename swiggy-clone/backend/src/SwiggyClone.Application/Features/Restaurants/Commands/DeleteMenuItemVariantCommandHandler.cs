using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

internal sealed class DeleteMenuItemVariantCommandHandler(IAppDbContext db)
    : IRequestHandler<DeleteMenuItemVariantCommand, Result>
{
    public async Task<Result> Handle(
        DeleteMenuItemVariantCommand request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (ownershipResult.IsFailure)
            return Result.Failure(ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var variant = await db.MenuItemVariants
            .FirstOrDefaultAsync(v => v.Id == request.VariantId && v.MenuItemId == request.MenuItemId, ct);

        if (variant is null)
            return Result.Failure("VARIANT_NOT_FOUND", "Menu item variant not found.");

        db.MenuItemVariants.Remove(variant);
        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
