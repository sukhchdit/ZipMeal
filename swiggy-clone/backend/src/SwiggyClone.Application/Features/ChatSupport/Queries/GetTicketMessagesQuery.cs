using MediatR;
using SwiggyClone.Application.Features.ChatSupport.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.ChatSupport.Queries;

public sealed record GetTicketMessagesQuery(
    Guid UserId,
    Guid TicketId,
    string? Cursor,
    int PageSize = 30) : IRequest<Result<CursorPagedResult<SupportMessageDto>>>;
