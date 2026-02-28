using MediatR;
using SwiggyClone.Application.Features.Discovery.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Favourites.Queries;

public sealed record GetFavouritesQuery(Guid UserId) : IRequest<Result<List<CustomerRestaurantDto>>>;
