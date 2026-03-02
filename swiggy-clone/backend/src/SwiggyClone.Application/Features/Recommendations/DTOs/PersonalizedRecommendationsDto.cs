namespace SwiggyClone.Application.Features.Recommendations.DTOs;

public sealed record PersonalizedRecommendationsDto(
    List<RecommendedRestaurantDto> RecommendedRestaurants,
    List<RecommendedMenuItemDto> RecommendedDishes);
