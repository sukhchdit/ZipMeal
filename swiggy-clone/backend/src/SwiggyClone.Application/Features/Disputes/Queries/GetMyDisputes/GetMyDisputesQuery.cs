using MediatR;
using SwiggyClone.Application.Features.Disputes.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Disputes.Queries.GetMyDisputes;

public sealed record GetMyDisputesQuery(
    Guid UserId,
    string? Cursor,
    int PageSize = 20) : IRequest<Result<CursorPagedResult<DisputeSummaryDto>>>;
