using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Application.Features.Admin.DTOs;

public sealed record AdminRestaurantDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    string? City,
    string? State,
    string OwnerName,
    string OwnerPhone,
    string? LogoUrl,
    RestaurantStatus Status,
    string? StatusReason,
    decimal AverageRating,
    int TotalRatings,
    bool IsAcceptingOrders,
    string? FssaiLicense,
    string? GstNumber,
    DateTimeOffset CreatedAt);
