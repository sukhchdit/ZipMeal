namespace SwiggyClone.Application.Features.Analytics.DTOs;

public sealed record MenuItemPerformanceDto(
    string ItemName,
    int TotalQuantitySold,
    decimal TotalRevenue,
    int OrderCount,
    double AvgRating);
