using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Commands;

public sealed record DeleteTableCommand(
    Guid OwnerId,
    Guid RestaurantId,
    Guid TableId) : IRequest<Result>;
