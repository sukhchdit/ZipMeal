using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.GroupOrders.Commands;

public sealed record LeaveGroupOrderCommand(
    Guid UserId,
    Guid GroupOrderId) : IRequest<Result>;
