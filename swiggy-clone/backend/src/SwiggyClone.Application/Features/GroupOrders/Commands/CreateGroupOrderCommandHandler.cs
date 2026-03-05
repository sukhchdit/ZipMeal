using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.GroupOrders.DTOs;
using SwiggyClone.Application.Features.GroupOrders.Notifications;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.GroupOrders.Commands;

internal sealed class CreateGroupOrderCommandHandler(IAppDbContext db, IPublisher publisher)
    : IRequestHandler<CreateGroupOrderCommand, Result<GroupOrderDto>>
{
    public async Task<Result<GroupOrderDto>> Handle(CreateGroupOrderCommand request, CancellationToken ct)
    {
        // 1. Validate restaurant
        var restaurant = await db.Restaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.RestaurantId, ct);

        if (restaurant is null || restaurant.Status != RestaurantStatus.Approved)
            return Result<GroupOrderDto>.Failure("RESTAURANT_NOT_FOUND", "Restaurant not found or not approved.");

        if (!restaurant.IsAcceptingOrders)
            return Result<GroupOrderDto>.Failure("RESTAURANT_CLOSED", "Restaurant is not currently accepting orders.");

        // 2. Check user has no active group order
        var hasActive = await db.GroupOrderParticipants
            .AnyAsync(p => p.UserId == request.UserId
                && p.Status != GroupOrderParticipantStatus.Left
                && p.GroupOrder.Status == GroupOrderStatus.Active, ct);

        if (hasActive)
            return Result<GroupOrderDto>.Failure("GROUP_ORDER_ALREADY_EXISTS",
                "You already have an active group order. Please leave or cancel it first.");

        // 3. Generate unique invite code
        var inviteCode = await GenerateUniqueInviteCodeAsync(ct);

        // 4. Create group order
        var now = DateTimeOffset.UtcNow;
        var user = await db.Users.AsNoTracking().FirstAsync(u => u.Id == request.UserId, ct);

        var groupOrder = new GroupOrder
        {
            Id = Guid.CreateVersion7(),
            RestaurantId = request.RestaurantId,
            InitiatorUserId = request.UserId,
            InviteCode = inviteCode,
            Status = GroupOrderStatus.Active,
            PaymentSplitType = request.PaymentSplitType,
            DeliveryAddressId = request.DeliveryAddressId,
            ExpiresAt = now.AddMinutes(60),
            CreatedAt = now,
            UpdatedAt = now,
            Participants =
            [
                new GroupOrderParticipant
                {
                    Id = Guid.CreateVersion7(),
                    UserId = request.UserId,
                    IsInitiator = true,
                    Status = GroupOrderParticipantStatus.Joined,
                    JoinedAt = now,
                }
            ],
        };

        db.GroupOrders.Add(groupOrder);
        await db.SaveChangesAsync(ct);

        await publisher.Publish(new GroupOrderCreatedNotification(
            groupOrder.Id, restaurant.Id, request.UserId, inviteCode), ct);

        // 5. Map to DTO
        var dto = new GroupOrderDto(
            groupOrder.Id,
            restaurant.Id,
            restaurant.Name,
            restaurant.LogoUrl,
            request.UserId,
            user.FullName,
            inviteCode,
            groupOrder.Status,
            groupOrder.PaymentSplitType,
            groupOrder.DeliveryAddressId,
            groupOrder.SpecialInstructions,
            groupOrder.ExpiresAt,
            groupOrder.FinalizedAt,
            groupOrder.OrderId,
            groupOrder.CreatedAt,
            groupOrder.Participants.Select(p => new GroupOrderParticipantDto(
                p.Id, p.UserId, user.FullName, user.AvatarUrl, p.IsInitiator,
                p.Status, p.JoinedAt, p.LeftAt, p.ItemCount, p.ItemsTotal)).ToList());

        return Result<GroupOrderDto>.Success(dto);
    }

    private async Task<string> GenerateUniqueInviteCodeAsync(CancellationToken ct)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        string code;
        do
        {
            code = new string(Enumerable.Range(0, 6)
                .Select(_ => chars[Random.Shared.Next(chars.Length)]).ToArray());
        } while (await db.GroupOrders.AnyAsync(g => g.InviteCode == code, ct));

        return code;
    }
}
