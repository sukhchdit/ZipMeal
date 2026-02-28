using MediatR;
using SwiggyClone.Application.Features.Admin.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Admin.Queries;

public sealed record GetAdminOrdersQuery(
    OrderStatus? StatusFilter,
    DateTimeOffset? FromDate,
    DateTimeOffset? ToDate,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PagedResult<AdminOrderSummaryDto>>>;
