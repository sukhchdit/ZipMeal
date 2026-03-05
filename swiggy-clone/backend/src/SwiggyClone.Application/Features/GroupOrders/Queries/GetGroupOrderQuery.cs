using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.GroupOrders.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.GroupOrders.Queries;

public sealed record GetGroupOrderQuery(
    Guid UserId,
    Guid GroupOrderId) : IRequest<Result<GroupOrderDto>>;

internal sealed class GetGroupOrderQueryHandler(IAppDbContext db)
    : IRequestHandler<GetGroupOrderQuery, Result<GroupOrderDto>>
{
    public async Task<Result<GroupOrderDto>> Handle(GetGroupOrderQuery request, CancellationToken ct)
    {
        var groupOrder = await db.GroupOrders
            .AsNoTracking()
            .Include(g => g.Restaurant)
            .Include(g => g.Participants)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(g => g.Id == request.GroupOrderId, ct);

        if (groupOrder is null)
            return Result<GroupOrderDto>.Failure("GROUP_ORDER_NOT_FOUND", "Group order not found.");

        var isParticipant = groupOrder.Participants.Any(p => p.UserId == request.UserId);
        if (!isParticipant)
            return Result<GroupOrderDto>.Failure("NOT_PARTICIPANT", "You are not a participant in this group order.");

        var initiator = groupOrder.Participants.First(p => p.IsInitiator);

        var dto = new GroupOrderDto(
            groupOrder.Id,
            groupOrder.RestaurantId,
            groupOrder.Restaurant.Name,
            groupOrder.Restaurant.LogoUrl,
            groupOrder.InitiatorUserId,
            initiator.User.FullName,
            groupOrder.InviteCode,
            groupOrder.Status,
            groupOrder.PaymentSplitType,
            groupOrder.DeliveryAddressId,
            groupOrder.SpecialInstructions,
            groupOrder.ExpiresAt,
            groupOrder.FinalizedAt,
            groupOrder.OrderId,
            groupOrder.CreatedAt,
            groupOrder.Participants.Select(p => new GroupOrderParticipantDto(
                p.Id, p.UserId, p.User.FullName, p.User.AvatarUrl, p.IsInitiator,
                p.Status, p.JoinedAt, p.LeftAt, p.ItemCount, p.ItemsTotal)).ToList());

        return Result<GroupOrderDto>.Success(dto);
    }
}
