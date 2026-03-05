using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.GroupOrders.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.GroupOrders.Queries;

public sealed record GetGroupCartQuery(
    Guid UserId,
    Guid GroupOrderId) : IRequest<Result<GroupCartDto>>;

internal sealed class GetGroupCartQueryHandler(
    IAppDbContext db,
    IGroupCartService groupCartService)
    : IRequestHandler<GetGroupCartQuery, Result<GroupCartDto>>
{
    public async Task<Result<GroupCartDto>> Handle(GetGroupCartQuery request, CancellationToken ct)
    {
        var isParticipant = await db.GroupOrderParticipants
            .AnyAsync(p => p.GroupOrderId == request.GroupOrderId
                && p.UserId == request.UserId
                && p.Status != GroupOrderParticipantStatus.Left, ct);

        if (!isParticipant)
            return Result<GroupCartDto>.Failure("NOT_PARTICIPANT", "You are not a participant in this group order.");

        var participantCarts = await groupCartService.GetAllParticipantCartsAsync(request.GroupOrderId, ct);

        // Get user names
        var userIds = participantCarts.Select(c => c.UserId).ToList();
        var users = await db.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.FullName, ct);

        var carts = participantCarts.Select(pc =>
        {
            users.TryGetValue(pc.UserId, out var userName);
            return new GroupParticipantCartDto(
                pc.UserId,
                userName ?? "Unknown",
                pc.Cart.Items,
                pc.Cart.Subtotal);
        }).ToList();

        var grandTotal = carts.Sum(c => c.Subtotal);

        return Result<GroupCartDto>.Success(new GroupCartDto(
            request.GroupOrderId, carts, grandTotal));
    }
}
