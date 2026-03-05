using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

public sealed class LoyaltyTier
{
    public Guid Id { get; set; }
    public LoyaltyTierLevel Level { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MinLifetimePoints { get; set; }
    public int PointsPerHundredPaise { get; set; }
    public double Multiplier { get; set; }
}
