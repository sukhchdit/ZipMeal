using MediatR;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Queries;

public sealed record GetMenuCategoriesQuery(Guid RestaurantId, Guid OwnerId) : IRequest<Result<List<MenuCategoryDto>>>;
