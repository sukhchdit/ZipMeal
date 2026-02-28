using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

public sealed record DeleteMenuItemAddonCommand(
    Guid RestaurantId,
    Guid OwnerId,
    Guid MenuItemId,
    Guid AddonId) : IRequest<Result>;
