using SwiggyClone.Domain.Common;

namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Singleton entity storing platform-wide configuration values such as delivery fees,
/// packaging charges, and tax rates. Only one row should exist in the database.
/// All monetary values are stored in paise (smallest currency unit).
/// </summary>
public sealed class PlatformConfig : BaseEntity
{
    /// <summary>
    /// Delivery fee in paise charged per order. Defaults to 4900 (₹49).
    /// </summary>
    public int DeliveryFeePaise { get; set; } = 4900;

    /// <summary>
    /// Packaging charge in paise added to each order. Defaults to 1500 (₹15).
    /// </summary>
    public int PackagingChargePaise { get; set; } = 1500;

    /// <summary>
    /// Tax rate percentage applied to the order subtotal. Defaults to 5.00 (5%).
    /// Stored as a decimal for precision (e.g., 5.00, 12.50, 18.00).
    /// </summary>
    public decimal TaxRatePercent { get; set; } = 5.00m;

    /// <summary>
    /// Order subtotal in paise above which delivery fee is waived.
    /// Null means free delivery is not available.
    /// </summary>
    public int? FreeDeliveryThresholdPaise { get; set; }

    /// <summary>
    /// Creates a new platform config with a UUID v7 identifier.
    /// </summary>
    public static PlatformConfig Create(
        int deliveryFeePaise,
        int packagingChargePaise,
        decimal taxRatePercent,
        int? freeDeliveryThresholdPaise)
    {
        var now = DateTimeOffset.UtcNow;
        return new PlatformConfig
        {
            Id = Guid.CreateVersion7(),
            DeliveryFeePaise = deliveryFeePaise,
            PackagingChargePaise = packagingChargePaise,
            TaxRatePercent = taxRatePercent,
            FreeDeliveryThresholdPaise = freeDeliveryThresholdPaise,
            CreatedAt = now,
            UpdatedAt = now,
        };
    }

    /// <summary>
    /// Updates the platform configuration values.
    /// </summary>
    public void Update(
        int deliveryFeePaise,
        int packagingChargePaise,
        decimal taxRatePercent,
        int? freeDeliveryThresholdPaise)
    {
        DeliveryFeePaise = deliveryFeePaise;
        PackagingChargePaise = packagingChargePaise;
        TaxRatePercent = taxRatePercent;
        FreeDeliveryThresholdPaise = freeDeliveryThresholdPaise;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
