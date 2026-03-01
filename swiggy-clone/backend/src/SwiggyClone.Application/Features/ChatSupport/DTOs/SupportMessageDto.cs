namespace SwiggyClone.Application.Features.ChatSupport.DTOs;

public sealed record SupportMessageDto(
    Guid Id,
    Guid TicketId,
    Guid SenderId,
    string SenderName,
    string Content,
    int MessageType,
    bool IsRead,
    DateTimeOffset? ReadAt,
    DateTimeOffset CreatedAt);
