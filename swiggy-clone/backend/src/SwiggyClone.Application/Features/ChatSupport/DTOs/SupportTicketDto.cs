namespace SwiggyClone.Application.Features.ChatSupport.DTOs;

public sealed record SupportTicketDto(
    Guid Id,
    Guid UserId,
    string UserName,
    Guid? AssignedAgentId,
    string? AssignedAgentName,
    string Subject,
    int Category,
    int Status,
    Guid? OrderId,
    string? LastMessage,
    DateTimeOffset? ClosedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
