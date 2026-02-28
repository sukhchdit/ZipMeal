using MediatR;
using SwiggyClone.Application.Features.FavouriteItems.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.FavouriteItems.Queries;

public sealed record GetFavouriteItemsQuery(Guid UserId) : IRequest<Result<List<FavouriteItemDto>>>;
