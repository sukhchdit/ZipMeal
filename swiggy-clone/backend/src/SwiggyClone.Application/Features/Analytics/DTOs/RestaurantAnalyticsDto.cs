namespace SwiggyClone.Application.Features.Analytics.DTOs;

public sealed record RestaurantAnalyticsDto(
    List<DataPointDto> RevenueTrend,
    List<DataPointDto> OrderTrend,
    List<NamedValueDto> TopMenuItems,
    List<DataPointDto> RatingTrend,
    List<NamedValueDto> OrderTypeDistribution,
    List<NamedValueDto> OrderStatusDistribution,
    List<DataPointDto> PeakHoursDistribution,
    decimal AverageOrderValue);
