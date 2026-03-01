using MediatR;
using SwiggyClone.Application.Features.ChatSupport.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.ChatSupport.Commands;

public sealed record SendMessageCommand(
    Guid UserId,
    Guid TicketId,
    string Content,
    int MessageType = 0) : IRequest<Result<SupportMessageDto>>;
