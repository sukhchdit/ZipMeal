namespace SwiggyClone.Application.Features.Admin.DTOs;

public sealed record AdminDashboardDto(
    UserCountsDto UserCounts,
    RestaurantCountsDto RestaurantCounts,
    OrderCountsDto OrderCounts,
    RevenueDto Revenue,
    List<AdminOrderSummaryDto> RecentOrders);
