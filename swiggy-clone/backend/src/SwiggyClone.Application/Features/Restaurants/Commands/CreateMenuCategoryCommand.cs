using MediatR;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

public sealed record CreateMenuCategoryCommand(
    Guid RestaurantId,
    Guid OwnerId,
    string Name,
    string? Description,
    int SortOrder) : IRequest<Result<MenuCategoryDto>>;
