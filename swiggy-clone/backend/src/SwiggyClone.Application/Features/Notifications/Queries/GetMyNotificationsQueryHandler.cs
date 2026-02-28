using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Notifications.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Notifications.Queries;

internal sealed class GetMyNotificationsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetMyNotificationsQuery, Result<CursorPagedResult<NotificationDto>>>
{
    public async Task<Result<CursorPagedResult<NotificationDto>>> Handle(
        GetMyNotificationsQuery request,
        CancellationToken ct)
    {
        var query = db.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == request.UserId)
            .OrderByDescending(n => n.CreatedAt);

        if (request.Cursor is not null &&
            DateTimeOffset.TryParse(request.Cursor, out var cursorDate))
        {
            query = (IOrderedQueryable<Domain.Entities.Notification>)
                query.Where(n => n.CreatedAt < cursorDate);
        }

        var pageSize = Math.Clamp(request.PageSize, 1, 50);

        var notifications = await query
            .Take(pageSize + 1)
            .Select(n => new NotificationDto(
                n.Id,
                n.Title,
                n.Body,
                (int)n.Type,
                n.Data,
                n.IsRead,
                n.ReadAt,
                n.CreatedAt))
            .ToListAsync(ct);

        var hasMore = notifications.Count > pageSize;
        if (hasMore) notifications.RemoveAt(notifications.Count - 1);

        var nextCursor = notifications.Count > 0
            ? notifications[^1].CreatedAt.ToString("O")
            : null;

        return Result<CursorPagedResult<NotificationDto>>.Success(
            new CursorPagedResult<NotificationDto>(
                notifications, nextCursor, null, hasMore, pageSize));
    }
}
