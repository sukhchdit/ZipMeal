using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.ChatSupport.Commands;

public sealed record CloseTicketCommand(
    Guid UserId,
    Guid TicketId) : IRequest<Result>;
