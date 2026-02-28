using MediatR;
using SwiggyClone.Application.Features.Notifications.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Notifications.Commands;

public sealed record SendNotificationCommand(
    Guid UserId,
    string Title,
    string Body,
    int Type,
    string? Data) : IRequest<Result<NotificationDto>>;
