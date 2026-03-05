using MediatR;
using SwiggyClone.Application.Features.GroupOrders.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.GroupOrders.Commands;

public sealed record JoinGroupOrderCommand(
    Guid UserId,
    string InviteCode) : IRequest<Result<GroupOrderDto>>;
