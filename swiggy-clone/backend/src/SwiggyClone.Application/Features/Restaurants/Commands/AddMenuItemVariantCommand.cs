using MediatR;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

public sealed record AddMenuItemVariantCommand(
    Guid RestaurantId,
    Guid OwnerId,
    Guid MenuItemId,
    string Name,
    int PriceAdjustment,
    bool IsDefault,
    bool IsAvailable,
    int SortOrder) : IRequest<Result<MenuItemVariantDto>>;
