namespace SwiggyClone.Api.Contracts.Disputes;

public sealed record CreateDisputeRequest(
    Guid OrderId,
    int IssueType,
    string Description);

public sealed record AddDisputeMessageRequest(
    string Content);

public sealed record ResolveDisputeRequest(
    int ResolutionType,
    int? ResolutionAmountPaise,
    string? ResolutionNotes);

public sealed record RejectDisputeRequest(
    string Reason);

public sealed record AssignDisputeRequest(
    Guid AgentId);
