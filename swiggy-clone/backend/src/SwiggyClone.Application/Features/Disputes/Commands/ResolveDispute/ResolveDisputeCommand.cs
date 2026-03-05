using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Disputes.Commands.ResolveDispute;

public sealed record ResolveDisputeCommand(
    Guid AgentId,
    Guid DisputeId,
    int ResolutionType,
    int? ResolutionAmountPaise,
    string? ResolutionNotes) : IRequest<Result>;
