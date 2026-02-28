using MediatR;
using SwiggyClone.Application.Features.Orders.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Orders.Queries;

public sealed record GetMyOrdersQuery(
    Guid UserId,
    string? Cursor,
    int PageSize = 20) : IRequest<Result<CursorPagedResult<OrderSummaryDto>>>;
