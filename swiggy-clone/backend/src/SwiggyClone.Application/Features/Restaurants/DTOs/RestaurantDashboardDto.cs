namespace SwiggyClone.Application.Features.Restaurants.DTOs;

public sealed record RestaurantDashboardDto(
    int TotalOrders,
    int PendingOrders,
    int TotalMenuItems,
    int ActiveMenuItems,
    decimal AverageRating,
    int TotalRatings,
    bool IsAcceptingOrders,
    string Status,
    int TotalTables,
    int ActiveTables,
    int ActiveSessions,
    int PendingDineInOrders);
