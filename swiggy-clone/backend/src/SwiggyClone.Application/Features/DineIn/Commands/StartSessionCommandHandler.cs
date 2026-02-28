using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Diagnostics;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.DineIn.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Application.Features.DineIn.Notifications;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Commands;

internal sealed class StartSessionCommandHandler(IAppDbContext db, IPublisher publisher)
    : IRequestHandler<StartSessionCommand, Result<DineInSessionDto>>
{
    public async Task<Result<DineInSessionDto>> Handle(StartSessionCommand request, CancellationToken ct)
    {
        // 1. Find table by QR code
        var table = await db.RestaurantTables
            .FirstOrDefaultAsync(t => t.QrCodeData == request.QrCodeData, ct);

        if (table is null)
            return Result<DineInSessionDto>.Failure("TABLE_NOT_FOUND", "No table found for the scanned QR code.");

        if (!table.IsActive)
            return Result<DineInSessionDto>.Failure("TABLE_INACTIVE", "This table is currently not available.");

        if (table.Status != TableStatus.Available)
            return Result<DineInSessionDto>.Failure("TABLE_OCCUPIED", "This table is already occupied or reserved.");

        // 2. Validate restaurant
        var restaurant = await db.Restaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == table.RestaurantId, ct);

        if (restaurant is null || restaurant.Status != RestaurantStatus.Approved)
            return Result<DineInSessionDto>.Failure("RESTAURANT_NOT_FOUND", "Restaurant not found or not approved.");

        if (!restaurant.IsDineInEnabled)
            return Result<DineInSessionDto>.Failure("DINEIN_DISABLED", "Dine-in is not enabled for this restaurant.");

        if (!restaurant.IsAcceptingOrders)
            return Result<DineInSessionDto>.Failure("RESTAURANT_CLOSED", "Restaurant is not currently accepting orders.");

        // 3. Check user has no active session (as host or guest)
        var hasActiveSession = await db.DineInSessionMembers
            .AnyAsync(m => m.UserId == request.UserId
                && m.Session.Status == DineInSessionStatus.Active, ct);

        if (hasActiveSession)
            return Result<DineInSessionDto>.Failure("ACTIVE_SESSION_EXISTS",
                "You already have an active dine-in session. Please end or leave it first.");

        // 4. Generate unique session code
        var sessionCode = await GenerateUniqueSessionCodeAsync(ct);

        // 5. Create session
        var now = DateTimeOffset.UtcNow;
        var user = await db.Users.AsNoTracking().FirstAsync(u => u.Id == request.UserId, ct);

        var session = new DineInSession
        {
            Id = Guid.CreateVersion7(),
            RestaurantId = table.RestaurantId,
            TableId = table.Id,
            CustomerId = request.UserId,
            SessionCode = sessionCode,
            Status = DineInSessionStatus.Active,
            GuestCount = request.GuestCount,
            StartedAt = now,
            CreatedAt = now,
            UpdatedAt = now,
            Members =
            [
                new DineInSessionMember
                {
                    Id = Guid.CreateVersion7(),
                    UserId = request.UserId,
                    Role = 1, // Host
                    JoinedAt = now,
                }
            ],
        };

        db.DineInSessions.Add(session);

        // 6. Mark table as occupied
        table.Status = TableStatus.Occupied;
        table.UpdatedAt = now;

        await db.SaveChangesAsync(ct);

        ApplicationDiagnostics.DineInSessionsStarted.Add(1);

        await publisher.Publish(new DineInSessionStartedNotification(
            session.Id, restaurant.Id, table.TableNumber), ct);

        // 7. Map to DTO
        var dto = new DineInSessionDto(
            session.Id,
            restaurant.Id,
            restaurant.Name,
            restaurant.LogoUrl,
            new DineInTableDto(table.Id, table.TableNumber, table.Capacity, table.FloorSection),
            session.SessionCode,
            session.Status,
            session.GuestCount,
            session.StartedAt,
            session.EndedAt,
            session.Members.Select(m => new DineInSessionMemberDto(
                m.UserId, user.FullName, user.AvatarUrl, m.Role, m.JoinedAt)).ToList(),
            []);

        return Result<DineInSessionDto>.Success(dto);
    }

    private async Task<string> GenerateUniqueSessionCodeAsync(CancellationToken ct)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Excludes ambiguous I,O,0,1
        string code;
        do
        {
            code = new string(Enumerable.Range(0, 6)
                .Select(_ => chars[Random.Shared.Next(chars.Length)]).ToArray());
        } while (await db.DineInSessions.AnyAsync(s => s.SessionCode == code, ct));

        return code;
    }
}
