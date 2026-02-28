using MediatR;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

public sealed record UpdateMenuItemAddonCommand(
    Guid RestaurantId,
    Guid OwnerId,
    Guid MenuItemId,
    Guid AddonId,
    string Name,
    int Price,
    bool IsVeg,
    bool IsAvailable,
    int MaxQuantity,
    int SortOrder) : IRequest<Result<MenuItemAddonDto>>;
