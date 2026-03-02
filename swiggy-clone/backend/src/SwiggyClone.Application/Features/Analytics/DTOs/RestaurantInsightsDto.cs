namespace SwiggyClone.Application.Features.Analytics.DTOs;

public sealed record RestaurantInsightsDto(
    int NewCustomers,
    int RepeatCustomers,
    double RepeatRate,
    List<DataPointDto> CustomerRetentionTrend,
    List<MenuItemPerformanceDto> MenuPerformance,
    double CompletionRate,
    double CancellationRate,
    List<DataPointDto> OrderCompletionTrend,
    List<NamedValueDto> RevenueByOrderType,
    List<NamedValueDto> RevenueByDayOfWeek,
    decimal AvgRevenuePerCustomer);
