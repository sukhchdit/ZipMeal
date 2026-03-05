using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.GroupOrders.Notifications;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.GroupOrders.Commands;

internal sealed class CancelGroupOrderCommandHandler(
    IAppDbContext db,
    IGroupCartService groupCartService,
    IPublisher publisher)
    : IRequestHandler<CancelGroupOrderCommand, Result>
{
    public async Task<Result> Handle(CancelGroupOrderCommand request, CancellationToken ct)
    {
        var groupOrder = await db.GroupOrders
            .FirstOrDefaultAsync(g => g.Id == request.GroupOrderId
                && g.Status == GroupOrderStatus.Active, ct);

        if (groupOrder is null)
            return Result.Failure("GROUP_ORDER_NOT_ACTIVE", "Group order is not active.");

        if (groupOrder.InitiatorUserId != request.UserId)
            return Result.Failure("NOT_INITIATOR", "Only the initiator can cancel the group order.");

        groupOrder.Status = GroupOrderStatus.Cancelled;
        groupOrder.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);
        await groupCartService.ClearAllCartsAsync(request.GroupOrderId, ct);

        await publisher.Publish(new GroupOrderCancelledNotification(request.GroupOrderId), ct);

        return Result.Success();
    }
}
