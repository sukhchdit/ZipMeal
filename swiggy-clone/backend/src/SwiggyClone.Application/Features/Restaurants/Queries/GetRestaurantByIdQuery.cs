using MediatR;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Queries;

public sealed record GetRestaurantByIdQuery(Guid RestaurantId, Guid OwnerId) : IRequest<Result<RestaurantDto>>;
