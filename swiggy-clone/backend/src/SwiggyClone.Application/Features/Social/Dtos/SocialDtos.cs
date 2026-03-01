namespace SwiggyClone.Application.Features.Social.Dtos;

public sealed record ActivityFeedItemDto(
    Guid Id,
    Guid UserId,
    string UserName,
    string? UserAvatarUrl,
    string ActivityType,
    Guid? TargetEntityId,
    string? Metadata,
    DateTimeOffset CreatedAt);

public sealed record ActivityFeedResponse(
    List<ActivityFeedItemDto> Items,
    DateTimeOffset? NextCursor,
    bool HasMore);

public sealed record UserProfileDto(
    Guid UserId,
    string FullName,
    string? AvatarUrl,
    int FollowerCount,
    int FollowingCount,
    int ReviewCount,
    bool IsFollowedByCurrentUser,
    List<ActivityFeedItemDto> RecentActivity);

public sealed record FollowUserDto(
    Guid UserId,
    string FullName,
    string? AvatarUrl,
    DateTimeOffset FollowedAt);

public sealed record FollowStatusDto(bool IsFollowing);

public sealed record ShareLinkDto(string ShareUrl, string ShareText);
