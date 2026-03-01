using MediatR;

namespace SwiggyClone.Application.Features.Social.Notifications;

public sealed record UserFollowedNotification(
    Guid FollowerId,
    Guid FollowingId) : INotification;
