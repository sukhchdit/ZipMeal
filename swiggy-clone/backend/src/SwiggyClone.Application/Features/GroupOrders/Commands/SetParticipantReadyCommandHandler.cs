using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.GroupOrders.Notifications;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.GroupOrders.Commands;

internal sealed class SetParticipantReadyCommandHandler(IAppDbContext db, IPublisher publisher)
    : IRequestHandler<SetParticipantReadyCommand, Result>
{
    public async Task<Result> Handle(SetParticipantReadyCommand request, CancellationToken ct)
    {
        var groupOrder = await db.GroupOrders
            .FirstOrDefaultAsync(g => g.Id == request.GroupOrderId
                && g.Status == GroupOrderStatus.Active, ct);

        if (groupOrder is null)
            return Result.Failure("GROUP_ORDER_NOT_ACTIVE", "Group order is not active.");

        var participant = await db.GroupOrderParticipants
            .FirstOrDefaultAsync(p => p.GroupOrderId == request.GroupOrderId
                && p.UserId == request.UserId
                && p.Status == GroupOrderParticipantStatus.Joined, ct);

        if (participant is null)
            return Result.Failure("NOT_PARTICIPANT", "You are not an active participant in this group order.");

        if (participant.Status == GroupOrderParticipantStatus.Ready)
            return Result.Failure("PARTICIPANT_ALREADY_READY", "You are already marked as ready.");

        participant.Status = GroupOrderParticipantStatus.Ready;
        groupOrder.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);

        await publisher.Publish(new GroupOrderParticipantReadyNotification(
            request.GroupOrderId, request.UserId), ct);

        return Result.Success();
    }
}
