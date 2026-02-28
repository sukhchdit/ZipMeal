using MediatR;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Discovery.Notifications;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

internal sealed class ToggleDineInCommandHandler(IAppDbContext db, IPublisher publisher)
    : IRequestHandler<ToggleDineInCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ToggleDineInCommand request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (ownershipResult.IsFailure)
            return Result<bool>.Failure(ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var restaurant = ownershipResult.Value;
        restaurant.IsDineInEnabled = request.IsDineInEnabled;
        restaurant.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);
        await publisher.Publish(new RestaurantIndexRequested(restaurant.Id), ct);

        return Result<bool>.Success(restaurant.IsDineInEnabled);
    }
}
