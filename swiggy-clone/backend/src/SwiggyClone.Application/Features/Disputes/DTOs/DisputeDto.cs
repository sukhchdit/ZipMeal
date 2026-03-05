namespace SwiggyClone.Application.Features.Disputes.DTOs;

public sealed record DisputeDto(
    Guid Id,
    string DisputeNumber,
    Guid OrderId,
    string? OrderNumber,
    Guid UserId,
    string UserName,
    Guid? AssignedAgentId,
    string? AssignedAgentName,
    int IssueType,
    int Status,
    string Description,
    int? ResolutionType,
    int? ResolutionAmountPaise,
    string? ResolutionNotes,
    DateTimeOffset? ResolvedAt,
    string? RejectionReason,
    DateTimeOffset? EscalatedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
