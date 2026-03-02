using SwiggyClone.Application.Features.Recommendations.DTOs;

namespace SwiggyClone.Application.Common.Interfaces;

public interface IRecommendationEngine
{
    Task<PersonalizedRecommendationsDto> GetPersonalizedAsync(
        Guid userId,
        string? city,
        int maxRestaurants = 10,
        int maxItems = 10,
        CancellationToken ct = default);

    Task<List<TrendingItemDto>> GetTrendingAsync(
        string? city,
        int count = 20,
        CancellationToken ct = default);

    Task<List<RecommendedRestaurantDto>> GetSimilarRestaurantsAsync(
        Guid restaurantId,
        int count = 10,
        CancellationToken ct = default);

    Task<List<RecommendedMenuItemDto>> GetSimilarItemsAsync(
        Guid menuItemId,
        int count = 10,
        CancellationToken ct = default);
}
