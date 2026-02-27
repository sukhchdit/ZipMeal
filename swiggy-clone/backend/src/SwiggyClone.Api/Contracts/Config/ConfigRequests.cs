namespace SwiggyClone.Api.Contracts.Config;

public sealed record UpdatePlatformConfigRequest(
    int DeliveryFeePaise,
    int PackagingChargePaise,
    decimal TaxRatePercent,
    int? FreeDeliveryThresholdPaise);
