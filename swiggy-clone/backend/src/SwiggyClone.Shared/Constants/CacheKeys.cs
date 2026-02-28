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

    public static string Restaurant(Guid id) => $"{RestaurantPrefix}{id}";
    public static string Menu(Guid restaurantId) => $"{MenuPrefix}{restaurantId}";
    public static string Cart(Guid userId) => $"{CartPrefix}{userId}";
    public static string UserSession(Guid userId) => $"{UserSessionPrefix}{userId}";
    public static string Otp(string phoneNumber) => $"{OtpPrefix}{phoneNumber}";
    public static string RateLimit(string key) => $"{RateLimitPrefix}{key}";
    public static string DineInSession(Guid sessionId) => $"{DineInSessionPrefix}{sessionId}";
}
