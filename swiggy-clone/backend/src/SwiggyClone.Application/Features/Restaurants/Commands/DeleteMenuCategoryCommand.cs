using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

public sealed record DeleteMenuCategoryCommand(
    Guid RestaurantId,
    Guid OwnerId,
    Guid CategoryId) : IRequest<Result>;
