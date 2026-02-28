using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

internal sealed class DeleteMenuItemAddonCommandHandler(IAppDbContext db)
    : IRequestHandler<DeleteMenuItemAddonCommand, Result>
{
    public async Task<Result> Handle(
        DeleteMenuItemAddonCommand request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (ownershipResult.IsFailure)
            return Result.Failure(ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var addon = await db.MenuItemAddons
            .FirstOrDefaultAsync(a => a.Id == request.AddonId && a.MenuItemId == request.MenuItemId, ct);

        if (addon is null)
            return Result.Failure("ADDON_NOT_FOUND", "Menu item addon not found.");

        db.MenuItemAddons.Remove(addon);
        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
