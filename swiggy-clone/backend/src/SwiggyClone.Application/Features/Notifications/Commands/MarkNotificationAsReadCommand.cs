using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Notifications.Commands;

public sealed record MarkNotificationAsReadCommand(
    Guid UserId,
    Guid NotificationId) : IRequest<Result>;
