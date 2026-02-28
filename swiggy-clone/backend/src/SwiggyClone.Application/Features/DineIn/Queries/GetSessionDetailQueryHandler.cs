using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.DineIn.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Queries;

internal sealed class GetSessionDetailQueryHandler(IAppDbContext db)
    : IRequestHandler<GetSessionDetailQuery, Result<DineInSessionDto>>
{
    public async Task<Result<DineInSessionDto>> Handle(
        GetSessionDetailQuery request, CancellationToken ct)
    {
        // 1. Validate user is a member
        var isMember = await db.DineInSessionMembers
            .AnyAsync(m => m.SessionId == request.SessionId
                && m.UserId == request.UserId, ct);

        if (!isMember)
            return Result<DineInSessionDto>.Failure("NOT_SESSION_MEMBER",
                "You are not a member of this dine-in session.");

        // 2. Load session with all related data
        var dto = await db.DineInSessions
            .AsNoTracking()
            .Where(s => s.Id == request.SessionId)
            .Select(s => new DineInSessionDto(
                s.Id,
                s.RestaurantId,
                s.Restaurant.Name,
                s.Restaurant.LogoUrl,
                new DineInTableDto(
                    s.Table.Id,
                    s.Table.TableNumber,
                    s.Table.Capacity,
                    s.Table.FloorSection),
                s.SessionCode,
                s.Status,
                s.GuestCount,
                s.StartedAt,
                s.EndedAt,
                s.Members
                    .OrderBy(m => m.JoinedAt)
                    .Select(m => new DineInSessionMemberDto(
                        m.UserId,
                        m.User.FullName,
                        m.User.AvatarUrl,
                        m.Role,
                        m.JoinedAt))
                    .ToList(),
                s.Orders
                    .OrderByDescending(o => o.CreatedAt)
                    .Select(o => new DineInOrderSummaryDto(
                        o.Id,
                        o.OrderNumber,
                        o.UserId,
                        o.User.FullName,
                        o.Status,
                        o.TotalAmount,
                        o.Items.Count,
                        o.CreatedAt))
                    .ToList()))
            .FirstOrDefaultAsync(ct);

        if (dto is null)
            return Result<DineInSessionDto>.Failure("SESSION_NOT_FOUND",
                "Dine-in session not found.");

        return Result<DineInSessionDto>.Success(dto);
    }
}
