using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Favourites.Commands;

public sealed record AddFavouriteCommand(Guid UserId, Guid RestaurantId) : IRequest<Result>;
