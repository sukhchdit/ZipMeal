using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.GroupOrders.Commands;

public sealed record CancelGroupOrderCommand(
    Guid UserId,
    Guid GroupOrderId) : IRequest<Result>;
