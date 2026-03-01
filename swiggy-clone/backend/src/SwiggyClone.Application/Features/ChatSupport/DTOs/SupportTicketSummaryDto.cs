namespace SwiggyClone.Application.Features.ChatSupport.DTOs;

public sealed record SupportTicketSummaryDto(
    Guid Id,
    string Subject,
    int Category,
    int Status,
    string? LastMessage,
    int UnreadCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
