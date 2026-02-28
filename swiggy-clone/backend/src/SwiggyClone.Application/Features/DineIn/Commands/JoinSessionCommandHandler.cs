using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.DineIn.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Application.Features.DineIn.Notifications;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Commands;

internal sealed class JoinSessionCommandHandler(IAppDbContext db, IPublisher publisher)
    : IRequestHandler<JoinSessionCommand, Result<DineInSessionDto>>
{
    public async Task<Result<DineInSessionDto>> Handle(JoinSessionCommand request, CancellationToken ct)
    {
        // 1. Find session by code
        var session = await db.DineInSessions
            .Include(s => s.Restaurant)
            .Include(s => s.Table)
            .Include(s => s.Members)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(s => s.SessionCode == request.SessionCode
                && s.Status == DineInSessionStatus.Active, ct);

        if (session is null)
            return Result<DineInSessionDto>.Failure("SESSION_NOT_FOUND",
                "No active session found with this code.");

        // 2. Check user is not already a member
        if (session.Members.Any(m => m.UserId == request.UserId))
            return Result<DineInSessionDto>.Failure("ALREADY_MEMBER",
                "You are already a member of this session.");

        // 3. Check user has no other active session
        var hasActiveSession = await db.DineInSessionMembers
            .AnyAsync(m => m.UserId == request.UserId
                && m.SessionId != session.Id
                && m.Session.Status == DineInSessionStatus.Active, ct);

        if (hasActiveSession)
            return Result<DineInSessionDto>.Failure("ACTIVE_SESSION_EXISTS",
                "You already have an active dine-in session. Please end or leave it first.");

        // 4. Add guest member
        var now = DateTimeOffset.UtcNow;
        var user = await db.Users.AsNoTracking().FirstAsync(u => u.Id == request.UserId, ct);

        var member = new DineInSessionMember
        {
            Id = Guid.CreateVersion7(),
            SessionId = session.Id,
            UserId = request.UserId,
            Role = 2, // Guest
            JoinedAt = now,
        };

        db.DineInSessionMembers.Add(member);
        session.GuestCount++;
        session.UpdatedAt = now;

        await db.SaveChangesAsync(ct);

        await publisher.Publish(new DineInMemberJoinedNotification(
            session.Id, request.UserId, user.FullName), ct);

        // 5. Load orders for the DTO
        var orders = await db.Orders
            .AsNoTracking()
            .Where(o => o.DineInSessionId == session.Id)
            .Select(o => new DineInOrderSummaryDto(
                o.Id,
                o.OrderNumber,
                o.UserId,
                o.User.FullName,
                o.Status,
                o.TotalAmount,
                o.Items.Count,
                o.CreatedAt))
            .ToListAsync(ct);

        // 6. Map to DTO (include the new member)
        var allMembers = session.Members
            .Append(new DineInSessionMember
            {
                UserId = request.UserId,
                Role = 2,
                JoinedAt = now,
                User = user
            })
            .DistinctBy(m => m.UserId)
            .Select(m => new DineInSessionMemberDto(
                m.UserId, m.User.FullName, m.User.AvatarUrl, m.Role, m.JoinedAt))
            .ToList();

        var dto = new DineInSessionDto(
            session.Id,
            session.RestaurantId,
            session.Restaurant.Name,
            session.Restaurant.LogoUrl,
            new DineInTableDto(session.Table.Id, session.Table.TableNumber,
                session.Table.Capacity, session.Table.FloorSection),
            session.SessionCode,
            session.Status,
            session.GuestCount,
            session.StartedAt,
            session.EndedAt,
            allMembers,
            orders);

        return Result<DineInSessionDto>.Success(dto);
    }
}
