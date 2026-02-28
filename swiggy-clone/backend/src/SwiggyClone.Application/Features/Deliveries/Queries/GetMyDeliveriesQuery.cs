using MediatR;
using SwiggyClone.Application.Features.Deliveries.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Deliveries.Queries;

public sealed record GetMyDeliveriesQuery(
    Guid PartnerId,
    string? Cursor,
    int PageSize = 20) : IRequest<Result<CursorPagedResult<DeliveryAssignmentDto>>>;
