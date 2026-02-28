using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Discovery.Notifications;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

internal sealed class DeleteMenuItemCommandHandler(IAppDbContext db, IPublisher publisher)
    : IRequestHandler<DeleteMenuItemCommand, Result>
{
    public async Task<Result> Handle(
        DeleteMenuItemCommand request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (ownershipResult.IsFailure)
            return Result.Failure(ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var menuItem = await db.MenuItems
            .FirstOrDefaultAsync(mi => mi.Id == request.MenuItemId && mi.RestaurantId == request.RestaurantId && !mi.IsDeleted, ct);

        if (menuItem is null)
            return Result.Failure("MENU_ITEM_NOT_FOUND", "Menu item not found.");

        menuItem.SoftDelete();
        await db.SaveChangesAsync(ct);
        await publisher.Publish(new MenuItemDeleteFromIndexRequested(menuItem.Id), ct);

        return Result.Success();
    }
}
