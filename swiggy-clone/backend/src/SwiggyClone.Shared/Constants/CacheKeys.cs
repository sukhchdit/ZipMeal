namespace SwiggyClone.Shared.Constants;

public static class CacheKeys
{
    public const string RestaurantPrefix = "restaurant:";
    public const string MenuPrefix = "menu:";
    public const string CartPrefix = "cart:";
    public const string UserSessionPrefix = "user_session:";
    public const string OtpPrefix = "otp:";
    public const string RateLimitPrefix = "rate_limit:";
    public const string DineInSessionPrefix = "dine_in_session:";
    public const string PlatformConfigKey = "platform_config";
    public const string AvailablePlansKey = "available_plans";
    public const string HomeFeedPrefix = "home_feed:";
    public const string CuisineTypesKey = "cuisine_types_all";
    public const string RecommendationsPersonalizedPrefix = "recommendations:personalized:";
    public const string RecommendationsTrendingPrefix = "recommendations:trending:";
    public const string RecommendationsSimilarRestaurantPrefix = "recommendations:similar_restaurant:";
    public const string RecommendationsSimilarItemPrefix = "recommendations:similar_item:";

    public static string Restaurant(Guid id) => $"{RestaurantPrefix}{id}";
    public static string RestaurantDetail(Guid id) => $"{RestaurantPrefix}detail:{id}";
    public static string HomeFeed(string? city) => $"{HomeFeedPrefix}{city ?? "_all_"}";
    public static string Menu(Guid restaurantId) => $"{MenuPrefix}{restaurantId}";
    public static string Cart(Guid userId) => $"{CartPrefix}{userId}";
    public static string UserSession(Guid userId) => $"{UserSessionPrefix}{userId}";
    public static string Otp(string phoneNumber) => $"{OtpPrefix}{phoneNumber}";
    public static string RateLimit(string key) => $"{RateLimitPrefix}{key}";
    public static string DineInSession(Guid sessionId) => $"{DineInSessionPrefix}{sessionId}";
    public static string RecommendationsPersonalized(Guid userId) => $"{RecommendationsPersonalizedPrefix}{userId}";
    public static string RecommendationsTrending(string? city) => $"{RecommendationsTrendingPrefix}{city ?? "_all_"}";
    public static string RecommendationsSimilarRestaurant(Guid restaurantId) => $"{RecommendationsSimilarRestaurantPrefix}{restaurantId}";
    public static string RecommendationsSimilarItem(Guid menuItemId) => $"{RecommendationsSimilarItemPrefix}{menuItemId}";
}
