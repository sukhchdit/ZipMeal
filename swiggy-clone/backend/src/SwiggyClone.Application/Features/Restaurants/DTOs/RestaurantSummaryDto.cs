namespace SwiggyClone.Application.Features.Restaurants.DTOs;

public sealed record RestaurantSummaryDto(
    Guid Id,
    string Name,
    string Slug,
    string? LogoUrl,
    string? City,
    decimal AverageRating,
    int TotalRatings,
    bool IsAcceptingOrders,
    string Status);
