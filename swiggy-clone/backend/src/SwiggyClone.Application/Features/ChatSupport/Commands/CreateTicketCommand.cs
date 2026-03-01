using MediatR;
using SwiggyClone.Application.Features.ChatSupport.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.ChatSupport.Commands;

public sealed record CreateTicketCommand(
    Guid UserId,
    string Subject,
    int Category,
    Guid? OrderId,
    string? InitialMessage) : IRequest<Result<SupportTicketDto>>;
