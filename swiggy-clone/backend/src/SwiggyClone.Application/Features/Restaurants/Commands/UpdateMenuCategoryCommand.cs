using MediatR;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

public sealed record UpdateMenuCategoryCommand(
    Guid RestaurantId,
    Guid OwnerId,
    Guid CategoryId,
    string Name,
    string? Description,
    int SortOrder,
    bool IsActive) : IRequest<Result<MenuCategoryDto>>;
