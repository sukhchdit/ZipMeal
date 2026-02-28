using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

public sealed record ToggleDineInCommand(
    Guid RestaurantId,
    Guid OwnerId,
    bool IsDineInEnabled) : IRequest<Result<bool>>;
