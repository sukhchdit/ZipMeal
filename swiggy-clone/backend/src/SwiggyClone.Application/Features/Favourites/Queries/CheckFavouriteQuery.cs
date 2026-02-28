using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Favourites.Queries;

public sealed record CheckFavouriteQuery(Guid UserId, Guid RestaurantId) : IRequest<Result<bool>>;
