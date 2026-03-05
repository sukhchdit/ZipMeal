namespace SwiggyClone.Application.Features.Loyalty.DTOs;

public sealed record LoyaltyDashboardDto(
    int PointsBalance,
    int LifetimePointsEarned,
    LoyaltyTierDto CurrentTier,
    LoyaltyTierDto? NextTier,
    int PointsToNextTier,
    IReadOnlyList<LoyaltyTransactionDto> RecentTransactions);

public sealed record LoyaltyTierDto(
    short Level,
    string Name,
    int MinLifetimePoints,
    double Multiplier);

public sealed record LoyaltyTransactionDto(
    Guid Id,
    int Points,
    short Type,
    short Source,
    Guid? ReferenceId,
    string Description,
    int BalanceAfter,
    DateTimeOffset CreatedAt);

public sealed record LoyaltyRewardDto(
    Guid Id,
    string Name,
    string? Description,
    int PointsCost,
    short RewardType,
    int RewardValue,
    int? Stock,
    DateTimeOffset? ExpiresAt);
