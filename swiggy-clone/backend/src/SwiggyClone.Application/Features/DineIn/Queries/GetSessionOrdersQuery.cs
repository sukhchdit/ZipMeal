using MediatR;
using SwiggyClone.Application.Features.Orders.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Queries;

public sealed record GetSessionOrdersQuery(
    Guid UserId,
    Guid SessionId) : IRequest<Result<List<OrderDto>>>;
