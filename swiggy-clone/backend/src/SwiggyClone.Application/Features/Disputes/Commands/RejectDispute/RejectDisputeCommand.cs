using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Disputes.Commands.RejectDispute;

public sealed record RejectDisputeCommand(
    Guid AgentId,
    Guid DisputeId,
    string Reason) : IRequest<Result>;
