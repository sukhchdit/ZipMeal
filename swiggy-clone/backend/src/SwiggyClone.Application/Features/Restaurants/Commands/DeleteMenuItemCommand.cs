using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

public sealed record DeleteMenuItemCommand(
    Guid RestaurantId,
    Guid OwnerId,
    Guid MenuItemId) : IRequest<Result>;
