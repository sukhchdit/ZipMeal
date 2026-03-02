namespace SwiggyClone.Application.Features.Analytics.DTOs;

public sealed record RevenueForecastDto(
    List<DataPointDto> HistoricalData,
    List<ForecastPointDto> ForecastData,
    decimal ProjectedTotalRevenue,
    decimal AvgDailyProjected,
    double GrowthRate);
