using SwiggyClone.Domain.Common;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

public sealed class Dispute : BaseEntity
{
    public string DisputeNumber { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public Guid? AssignedAgentId { get; set; }
    public DisputeIssueType IssueType { get; set; }
    public DisputeStatus Status { get; set; }
    public string Description { get; set; } = string.Empty;

    // Resolution
    public DisputeResolutionType? ResolutionType { get; set; }
    public int? ResolutionAmountPaise { get; set; }
    public string? ResolutionNotes { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }
    public Guid? ResolvedByAgentId { get; set; }
    public string? RejectionReason { get; set; }
    public DateTimeOffset? EscalatedAt { get; set; }

    // Navigation
    public Order Order { get; set; } = null!;
    public User User { get; set; } = null!;
    public User? AssignedAgent { get; set; }
    public User? ResolvedByAgent { get; set; }
    public ICollection<DisputeMessage> Messages { get; set; } = [];
}
