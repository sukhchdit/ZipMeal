using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Orders.Commands;

public sealed record CancelOrderCommand(
    Guid UserId,
    Guid OrderId,
    string? CancellationReason) : IRequest<Result>;
