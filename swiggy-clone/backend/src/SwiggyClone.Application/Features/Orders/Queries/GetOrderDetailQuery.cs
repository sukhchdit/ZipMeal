using MediatR;
using SwiggyClone.Application.Features.Orders.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Orders.Queries;

public sealed record GetOrderDetailQuery(Guid UserId, Guid OrderId) : IRequest<Result<OrderDto>>;
