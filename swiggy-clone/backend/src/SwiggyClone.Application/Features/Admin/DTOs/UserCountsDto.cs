namespace SwiggyClone.Application.Features.Admin.DTOs;

public sealed record UserCountsDto(
    int Total,
    int Customers,
    int RestaurantOwners,
    int DeliveryPartners,
    int Admins);
