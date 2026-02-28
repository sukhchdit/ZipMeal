using MediatR;
using SwiggyClone.Application.Features.Notifications.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Notifications.Queries;

public sealed record GetUnreadCountQuery(Guid UserId)
    : IRequest<Result<UnreadCountDto>>;
