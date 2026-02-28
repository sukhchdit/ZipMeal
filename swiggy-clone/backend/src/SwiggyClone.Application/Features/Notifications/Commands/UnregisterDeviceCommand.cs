using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Notifications.Commands;

public sealed record UnregisterDeviceCommand(
    Guid UserId,
    string DeviceToken) : IRequest<Result>;
