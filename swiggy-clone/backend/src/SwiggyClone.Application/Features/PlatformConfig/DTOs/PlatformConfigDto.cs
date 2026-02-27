namespace SwiggyClone.Application.Features.PlatformConfig.DTOs;

public sealed record PlatformConfigDto(
    Guid Id,
    int DeliveryFeePaise,
    int PackagingChargePaise,
    decimal TaxRatePercent,
    int? FreeDeliveryThresholdPaise,
    DateTimeOffset UpdatedAt);
