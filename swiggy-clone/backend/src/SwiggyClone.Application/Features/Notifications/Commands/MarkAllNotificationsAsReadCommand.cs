using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Notifications.Commands;

public sealed record MarkAllNotificationsAsReadCommand(Guid UserId) : IRequest<Result>;
