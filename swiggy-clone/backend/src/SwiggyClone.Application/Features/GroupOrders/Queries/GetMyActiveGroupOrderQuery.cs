using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.GroupOrders.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.GroupOrders.Queries;

public sealed record GetMyActiveGroupOrderQuery(Guid UserId) : IRequest<Result<GroupOrderDto?>>;

internal sealed class GetMyActiveGroupOrderQueryHandler(IAppDbContext db)
    : IRequestHandler<GetMyActiveGroupOrderQuery, Result<GroupOrderDto?>>
{
    public async Task<Result<GroupOrderDto?>> Handle(GetMyActiveGroupOrderQuery request, CancellationToken ct)
    {
        var participant = await db.GroupOrderParticipants
            .AsNoTracking()
            .Include(p => p.GroupOrder)
                .ThenInclude(g => g.Restaurant)
            .Include(p => p.GroupOrder)
                .ThenInclude(g => g.Participants)
                    .ThenInclude(p2 => p2.User)
            .Where(p => p.UserId == request.UserId
                && p.Status != GroupOrderParticipantStatus.Left
                && p.GroupOrder.Status == GroupOrderStatus.Active)
            .FirstOrDefaultAsync(ct);

        if (participant is null)
            return Result<GroupOrderDto?>.Success(null);

        var groupOrder = participant.GroupOrder;
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

        return Result<GroupOrderDto?>.Success(dto);
    }
}
