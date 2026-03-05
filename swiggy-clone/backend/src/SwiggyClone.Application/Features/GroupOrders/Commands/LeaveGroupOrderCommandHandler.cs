using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.GroupOrders.Notifications;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.GroupOrders.Commands;

internal sealed class LeaveGroupOrderCommandHandler(
    IAppDbContext db,
    IGroupCartService groupCartService,
    IPublisher publisher)
    : IRequestHandler<LeaveGroupOrderCommand, Result>
{
    public async Task<Result> Handle(LeaveGroupOrderCommand request, CancellationToken ct)
    {
        var groupOrder = await db.GroupOrders
            .FirstOrDefaultAsync(g => g.Id == request.GroupOrderId
                && g.Status == GroupOrderStatus.Active, ct);

        if (groupOrder is null)
            return Result.Failure("GROUP_ORDER_NOT_FOUND", "No active group order found.");

        var participant = await db.GroupOrderParticipants
            .FirstOrDefaultAsync(p => p.GroupOrderId == request.GroupOrderId
                && p.UserId == request.UserId
                && p.Status != GroupOrderParticipantStatus.Left, ct);

        if (participant is null)
            return Result.Failure("NOT_PARTICIPANT", "You are not an active participant in this group order.");

        if (participant.IsInitiator)
            return Result.Failure("INITIATOR_CANNOT_LEAVE",
                "The initiator cannot leave. Please cancel the group order instead.");

        participant.Status = GroupOrderParticipantStatus.Left;
        participant.LeftAt = DateTimeOffset.UtcNow;
        groupOrder.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);
        await groupCartService.ClearParticipantCartAsync(request.GroupOrderId, request.UserId, ct);

        await publisher.Publish(new GroupOrderParticipantLeftNotification(
            request.GroupOrderId, request.UserId), ct);

        return Result.Success();
    }
}
