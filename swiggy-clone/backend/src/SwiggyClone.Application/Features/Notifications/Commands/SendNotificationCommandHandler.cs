using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Notifications.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Notifications.Commands;

internal sealed class SendNotificationCommandHandler(
    IAppDbContext db,
    INotificationService notificationService)
    : IRequestHandler<SendNotificationCommand, Result<NotificationDto>>
{
    public async Task<Result<NotificationDto>> Handle(
        SendNotificationCommand request,
        CancellationToken ct)
    {
        var notification = new Notification
        {
            Id = Guid.CreateVersion7(),
            UserId = request.UserId,
            Title = request.Title,
            Body = request.Body,
            Type = (NotificationType)request.Type,
            Data = request.Data,
            IsRead = false,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        db.Notifications.Add(notification);
        await db.SaveChangesAsync(ct);

        // Fire-and-forget push (best-effort)
        _ = notificationService.SendPushAsync(
            request.UserId, request.Title, request.Body, request.Data, ct);

        return Result<NotificationDto>.Success(new NotificationDto(
            notification.Id,
            notification.Title,
            notification.Body,
            (int)notification.Type,
            notification.Data,
            notification.IsRead,
            notification.ReadAt,
            notification.CreatedAt));
    }
}
