using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.GroupOrders.DTOs;
using SwiggyClone.Application.Features.GroupOrders.Notifications;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.GroupOrders.Commands;

internal sealed class JoinGroupOrderCommandHandler(IAppDbContext db, IPublisher publisher)
    : IRequestHandler<JoinGroupOrderCommand, Result<GroupOrderDto>>
{
    public async Task<Result<GroupOrderDto>> Handle(JoinGroupOrderCommand request, CancellationToken ct)
    {
        // 1. Find group order by invite code
        var groupOrder = await db.GroupOrders
            .Include(g => g.Restaurant)
            .Include(g => g.Participants)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(g => g.InviteCode == request.InviteCode
                && g.Status == GroupOrderStatus.Active, ct);

        if (groupOrder is null)
            return Result<GroupOrderDto>.Failure("GROUP_ORDER_NOT_FOUND",
                "No active group order found with this code.");

        // 2. Check expiry
        if (groupOrder.ExpiresAt <= DateTimeOffset.UtcNow)
            return Result<GroupOrderDto>.Failure("GROUP_ORDER_EXPIRED",
                "This group order has expired.");

        // 3. Check user is not already a participant
        if (groupOrder.Participants.Any(p => p.UserId == request.UserId && p.Status != GroupOrderParticipantStatus.Left))
            return Result<GroupOrderDto>.Failure("ALREADY_PARTICIPANT",
                "You are already a participant in this group order.");

        // 4. Check user has no other active group order
        var hasActive = await db.GroupOrderParticipants
            .AnyAsync(p => p.UserId == request.UserId
                && p.GroupOrderId != groupOrder.Id
                && p.Status != GroupOrderParticipantStatus.Left
                && p.GroupOrder.Status == GroupOrderStatus.Active, ct);

        if (hasActive)
            return Result<GroupOrderDto>.Failure("GROUP_ORDER_ALREADY_EXISTS",
                "You already have an active group order. Please leave or cancel it first.");

        // 5. Check max participants (10)
        var activeCount = groupOrder.Participants.Count(p => p.Status != GroupOrderParticipantStatus.Left);
        if (activeCount >= 10)
            return Result<GroupOrderDto>.Failure("GROUP_ORDER_FULL",
                "This group order has reached the maximum number of participants.");

        // 6. Add participant
        var now = DateTimeOffset.UtcNow;
        var user = await db.Users.AsNoTracking().FirstAsync(u => u.Id == request.UserId, ct);

        var participant = new GroupOrderParticipant
        {
            Id = Guid.CreateVersion7(),
            GroupOrderId = groupOrder.Id,
            UserId = request.UserId,
            IsInitiator = false,
            Status = GroupOrderParticipantStatus.Joined,
            JoinedAt = now,
        };

        db.GroupOrderParticipants.Add(participant);
        groupOrder.UpdatedAt = now;

        await db.SaveChangesAsync(ct);

        await publisher.Publish(new GroupOrderParticipantJoinedNotification(
            groupOrder.Id, request.UserId, user.FullName), ct);

        // 7. Map to DTO
        var initiator = await db.Users.AsNoTracking()
            .FirstAsync(u => u.Id == groupOrder.InitiatorUserId, ct);

        var allParticipants = groupOrder.Participants
            .Append(participant)
            .DistinctBy(p => p.Id)
            .Select(p =>
            {
                var pUser = p.UserId == request.UserId ? user : p.User;
                return new GroupOrderParticipantDto(
                    p.Id, p.UserId, pUser.FullName, pUser.AvatarUrl, p.IsInitiator,
                    p.Status, p.JoinedAt, p.LeftAt, p.ItemCount, p.ItemsTotal);
            })
            .ToList();

        var dto = new GroupOrderDto(
            groupOrder.Id,
            groupOrder.RestaurantId,
            groupOrder.Restaurant.Name,
            groupOrder.Restaurant.LogoUrl,
            groupOrder.InitiatorUserId,
            initiator.FullName,
            groupOrder.InviteCode,
            groupOrder.Status,
            groupOrder.PaymentSplitType,
            groupOrder.DeliveryAddressId,
            groupOrder.SpecialInstructions,
            groupOrder.ExpiresAt,
            groupOrder.FinalizedAt,
            groupOrder.OrderId,
            groupOrder.CreatedAt,
            allParticipants);

        return Result<GroupOrderDto>.Success(dto);
    }
}
