namespace SwiggyClone.Application.Features.Analytics.DTOs;

public sealed record PartnerAnalyticsDto(
    List<DataPointDto> EarningsTrend,
    List<DataPointDto> DeliveryCountTrend,
    double AvgDeliveryTimeMinutes,
    double CompletionRatePercent,
    int TotalDeliveries,
    long TotalEarnings,
    double AvgRating);
