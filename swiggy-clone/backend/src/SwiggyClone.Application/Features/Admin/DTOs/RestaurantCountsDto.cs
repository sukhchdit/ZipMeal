namespace SwiggyClone.Application.Features.Admin.DTOs;

public sealed record RestaurantCountsDto(
    int Total,
    int Pending,
    int Approved,
    int Suspended,
    int Rejected);
