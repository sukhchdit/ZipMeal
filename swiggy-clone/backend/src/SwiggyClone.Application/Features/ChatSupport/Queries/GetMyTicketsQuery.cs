using MediatR;
using SwiggyClone.Application.Features.ChatSupport.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.ChatSupport.Queries;

public sealed record GetMyTicketsQuery(
    Guid UserId,
    string? Cursor,
    int PageSize = 20) : IRequest<Result<CursorPagedResult<SupportTicketSummaryDto>>>;
