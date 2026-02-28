namespace SwiggyClone.Application.Features.Notifications.DTOs;

public sealed record NotificationDto(
    Guid Id,
    string Title,
    string Body,
    int Type,
    string? Data,
    bool IsRead,
    DateTimeOffset? ReadAt,
    DateTimeOffset CreatedAt);
