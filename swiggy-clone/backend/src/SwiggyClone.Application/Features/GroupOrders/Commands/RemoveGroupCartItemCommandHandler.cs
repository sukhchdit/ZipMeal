using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Cart.DTOs;
using SwiggyClone.Application.Features.GroupOrders.Notifications;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.GroupOrders.Commands;

internal sealed class RemoveGroupCartItemCommandHandler(
    IAppDbContext db,
    IGroupCartService groupCartService,
    IPublisher publisher)
    : IRequestHandler<RemoveGroupCartItemCommand, Result<CartDto>>
{
    public async Task<Result<CartDto>> Handle(RemoveGroupCartItemCommand request, CancellationToken ct)
    {
        var groupOrder = await db.GroupOrders
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == request.GroupOrderId
                && g.Status == GroupOrderStatus.Active, ct);

        if (groupOrder is null)
            return Result<CartDto>.Failure("GROUP_ORDER_NOT_ACTIVE", "Group order is not active.");

        var isParticipant = await db.GroupOrderParticipants
            .AnyAsync(p => p.GroupOrderId == request.GroupOrderId
                && p.UserId == request.UserId
                && p.Status != GroupOrderParticipantStatus.Left, ct);

        if (!isParticipant)
            return Result<CartDto>.Failure("NOT_PARTICIPANT", "You are not a participant in this group order.");

        var result = await groupCartService.RemoveItemAsync(
            request.GroupOrderId, request.UserId, request.CartItemId, ct);

        if (result.IsSuccess)
        {
            var participant = await db.GroupOrderParticipants
                .FirstAsync(p => p.GroupOrderId == request.GroupOrderId && p.UserId == request.UserId, ct);
            participant.ItemCount = result.Value.Items.Count;
            participant.ItemsTotal = result.Value.Subtotal;
            await db.SaveChangesAsync(ct);

            await publisher.Publish(new GroupOrderCartUpdatedNotification(
                request.GroupOrderId, request.UserId, result.Value.Items.Count, result.Value.Subtotal), ct);
        }

        return result;
    }
}
