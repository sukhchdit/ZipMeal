using MediatR;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Commands;

public sealed record UpdateDineInOrderStatusCommand(
    Guid OwnerId,
    Guid OrderId,
    DineInOrderStatus NewStatus,
    string? Notes) : IRequest<Result>;
