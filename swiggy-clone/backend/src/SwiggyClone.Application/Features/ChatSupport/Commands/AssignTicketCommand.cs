using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.ChatSupport.Commands;

public sealed record AssignTicketCommand(
    Guid TicketId,
    Guid AgentId) : IRequest<Result>;
