using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

public sealed record ToggleAcceptingOrdersCommand(
    Guid RestaurantId,
    Guid OwnerId,
    bool IsAcceptingOrders) : IRequest<Result<bool>>;
