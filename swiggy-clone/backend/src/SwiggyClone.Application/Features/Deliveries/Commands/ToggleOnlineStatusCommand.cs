using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Deliveries.Commands;

public sealed record ToggleOnlineStatusCommand(
    Guid PartnerId,
    bool IsOnline,
    double? Latitude,
    double? Longitude) : IRequest<Result>;
