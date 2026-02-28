using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Notifications.Commands;

public sealed record RegisterDeviceCommand(
    Guid UserId,
    string DeviceToken,
    int Platform) : IRequest<Result>;
