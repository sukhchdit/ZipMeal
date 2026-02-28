using MediatR;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Queries;

public sealed record GetMenuItemByIdQuery(Guid MenuItemId, Guid RestaurantId, Guid OwnerId)
    : IRequest<Result<MenuItemDto>>;
