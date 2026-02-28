using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Favourites.Commands;

public sealed record RemoveFavouriteCommand(Guid UserId, Guid RestaurantId) : IRequest<Result>;
