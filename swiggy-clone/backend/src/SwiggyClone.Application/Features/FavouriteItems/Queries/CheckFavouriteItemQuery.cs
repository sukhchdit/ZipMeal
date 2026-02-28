using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.FavouriteItems.Queries;

public sealed record CheckFavouriteItemQuery(Guid UserId, Guid MenuItemId) : IRequest<Result<bool>>;
