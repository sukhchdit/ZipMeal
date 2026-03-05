using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Disputes.Commands.AssignDispute;

public sealed record AssignDisputeCommand(
    Guid DisputeId,
    Guid AgentId) : IRequest<Result>;
