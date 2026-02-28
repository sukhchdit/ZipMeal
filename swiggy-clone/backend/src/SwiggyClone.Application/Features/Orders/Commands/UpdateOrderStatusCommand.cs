using MediatR;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Orders.Commands;

public sealed record UpdateOrderStatusCommand(
    Guid UserId,
    Guid OrderId,
    OrderStatus NewStatus,
    string? Notes) : IRequest<Result>;
