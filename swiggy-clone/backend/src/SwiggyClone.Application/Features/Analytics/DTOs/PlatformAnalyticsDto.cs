namespace SwiggyClone.Application.Features.Analytics.DTOs;

public sealed record PlatformAnalyticsDto(
    List<DataPointDto> RevenueTrend,
    List<DataPointDto> OrderTrend,
    List<DataPointDto> UserGrowthTrend,
    List<NamedValueDto> OrderStatusDistribution,
    List<NamedValueDto> OrderTypeDistribution,
    List<NamedValueDto> PaymentMethodDistribution,
    List<NamedValueDto> TopRestaurantsByRevenue,
    List<NamedValueDto> TopRestaurantsByOrders,
    List<NamedValueDto> PopularMenuItems,
    CouponAnalyticsDto CouponStats,
    DeliveryPerformanceDto DeliveryPerformance,
    List<DataPointDto> PeakHoursDistribution);
