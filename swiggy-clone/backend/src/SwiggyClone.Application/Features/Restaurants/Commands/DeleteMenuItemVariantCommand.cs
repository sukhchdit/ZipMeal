using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

public sealed record DeleteMenuItemVariantCommand(
    Guid RestaurantId,
    Guid OwnerId,
    Guid MenuItemId,
    Guid VariantId) : IRequest<Result>;
