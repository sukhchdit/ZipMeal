using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

public sealed class LoyaltyReward
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int PointsCost { get; set; }
    public LoyaltyRewardType RewardType { get; set; }
    public int RewardValue { get; set; }
    public bool IsActive { get; set; }
    public int? Stock { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public bool IsRedeemable()
    {
        if (!IsActive) return false;
        if (ExpiresAt.HasValue && ExpiresAt.Value < DateTimeOffset.UtcNow) return false;
        if (Stock.HasValue && Stock.Value <= 0) return false;
        return true;
    }

    public void DecrementStock()
    {
        if (Stock.HasValue)
        {
            Stock--;
            UpdatedAt = DateTimeOffset.UtcNow;
        }
    }
}
