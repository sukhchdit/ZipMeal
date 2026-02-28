namespace SwiggyClone.Application.Features.Deliveries.DTOs;

public sealed record PartnerDashboardDto(
    bool IsOnline,
    int TotalDeliveries,
    int TodayDeliveries,
    int TodayEarnings,
    int TotalEarnings);
