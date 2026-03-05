using MediatR;
using SwiggyClone.Application.Features.Disputes.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Disputes.Queries.GetAllDisputes;

public sealed record GetAllDisputesQuery(
    int? Status,
    int? IssueType,
    Guid? AgentId,
    string? Cursor,
    int PageSize = 20) : IRequest<Result<CursorPagedResult<DisputeSummaryDto>>>;
