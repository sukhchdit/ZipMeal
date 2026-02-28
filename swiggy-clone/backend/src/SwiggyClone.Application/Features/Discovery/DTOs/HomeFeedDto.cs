namespace SwiggyClone.Application.Features.Discovery.DTOs;

/// <summary>
/// Aggregated home feed response containing banners, cuisine categories,
/// and featured restaurant sections.
/// </summary>
public sealed record HomeFeedDto(
    List<BannerDto> Banners,
    List<CuisineChipDto> CuisineChips,
    List<RestaurantSectionDto> Sections);

public sealed record BannerDto(
    string Id,
    string ImageUrl,
    string? DeepLink);

public sealed record CuisineChipDto(
    Guid Id,
    string Name,
    string? IconUrl);

/// <summary>
/// A labelled horizontal list of restaurants (e.g. "Popular Near You", "Top Rated").
/// </summary>
public sealed record RestaurantSectionDto(
    string Title,
    List<CustomerRestaurantDto> Restaurants);
