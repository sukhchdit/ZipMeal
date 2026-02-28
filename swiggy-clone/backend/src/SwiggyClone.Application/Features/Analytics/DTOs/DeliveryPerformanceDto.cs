namespace SwiggyClone.Application.Features.Analytics.DTOs;

public sealed record DeliveryPerformanceDto(
    double AvgDeliveryTimeMinutes,
    double CompletionRatePercent,
    int TotalDeliveries,
    int CancelledDeliveries);
