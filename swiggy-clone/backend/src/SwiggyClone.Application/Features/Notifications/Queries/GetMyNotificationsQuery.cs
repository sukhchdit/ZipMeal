using MediatR;
using SwiggyClone.Application.Features.Notifications.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Notifications.Queries;

public sealed record GetMyNotificationsQuery(
    Guid UserId,
    string? Cursor,
    int PageSize = 20) : IRequest<Result<CursorPagedResult<NotificationDto>>>;
