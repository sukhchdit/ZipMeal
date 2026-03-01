using MediatR;
using SwiggyClone.Application.Features.ChatSupport.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.ChatSupport.Queries;

public sealed record GetAllTicketsQuery(
    int? Status,
    int? Category,
    Guid? AgentId,
    string? Cursor,
    int PageSize = 20) : IRequest<Result<CursorPagedResult<SupportTicketDto>>>;
