namespace SwiggyClone.Api.Contracts.Loyalty;

public sealed record RedeemRewardRequest(Guid RewardId);

public sealed record AdjustPointsRequest(Guid UserId, int Points, string Description);

public sealed record CreateRewardRequest(
    string Name,
    string? Description,
    int PointsCost,
    short RewardType,
    int RewardValue,
    int? Stock,
    DateTimeOffset? ExpiresAt);

public sealed record UpdateRewardRequest(
    string Name,
    string? Description,
    int PointsCost,
    short RewardType,
    int RewardValue,
    bool IsActive,
    int? Stock,
    DateTimeOffset? ExpiresAt);
