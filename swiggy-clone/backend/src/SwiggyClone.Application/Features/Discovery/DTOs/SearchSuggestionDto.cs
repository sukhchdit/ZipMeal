namespace SwiggyClone.Application.Features.Discovery.DTOs;

/// <summary>
/// A single autocomplete suggestion, which can be either a restaurant or a dish.
/// </summary>
public sealed record SearchSuggestionDto(
    string Text,
    string Type,
    Guid Id,
    Guid? RestaurantId,
    string? RestaurantName,
    string? ImageUrl);
