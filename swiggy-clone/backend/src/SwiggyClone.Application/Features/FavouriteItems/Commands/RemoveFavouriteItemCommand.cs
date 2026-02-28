using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.FavouriteItems.Commands;

public sealed record RemoveFavouriteItemCommand(Guid UserId, Guid MenuItemId) : IRequest<Result>;
