namespace SwiggyClone.Application.Features.Disputes.DTOs;

public sealed record DisputeSummaryDto(
    Guid Id,
    string DisputeNumber,
    string? OrderNumber,
    int IssueType,
    int Status,
    string? LastMessage,
    int UnreadCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
