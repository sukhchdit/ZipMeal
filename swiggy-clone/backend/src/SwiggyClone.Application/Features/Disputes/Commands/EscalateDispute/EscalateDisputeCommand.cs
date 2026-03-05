using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Disputes.Commands.EscalateDispute;

public sealed record EscalateDisputeCommand(
    Guid DisputeId) : IRequest<Result>;
