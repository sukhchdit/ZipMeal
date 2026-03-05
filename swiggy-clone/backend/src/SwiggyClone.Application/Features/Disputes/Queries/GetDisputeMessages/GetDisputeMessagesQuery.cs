using MediatR;
using SwiggyClone.Application.Features.Disputes.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Disputes.Queries.GetDisputeMessages;

public sealed record GetDisputeMessagesQuery(
    Guid UserId,
    Guid DisputeId,
    string? Cursor,
    int PageSize = 30) : IRequest<Result<CursorPagedResult<DisputeMessageDto>>>;
