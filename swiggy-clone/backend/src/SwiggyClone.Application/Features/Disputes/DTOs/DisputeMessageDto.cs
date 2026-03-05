namespace SwiggyClone.Application.Features.Disputes.DTOs;

public sealed record DisputeMessageDto(
    Guid Id,
    Guid DisputeId,
    Guid SenderId,
    string SenderName,
    string Content,
    bool IsSystemMessage,
    bool IsRead,
    DateTimeOffset? ReadAt,
    DateTimeOffset CreatedAt);
