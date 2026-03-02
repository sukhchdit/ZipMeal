using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Recommendations.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Infrastructure.Persistence;

namespace SwiggyClone.Infrastructure.Services;

internal sealed class RecommendationEngine(AppDbContext db) : IRecommendationEngine
{
    private static readonly string[] BreakfastKeywords = ["breakfast", "morning", "brunch", "idli", "dosa", "poha", "paratha"];
    private static readonly string[] LunchKeywords = ["lunch", "thali", "meals", "rice", "biryani"];
    private static readonly string[] DinnerKeywords = ["dinner", "tandoori", "kebab", "curry"];
    private static readonly string[] SnackKeywords = ["snack", "chaat", "samosa", "tea", "coffee", "shake"];

    private const int ColdStartThreshold = 3;
    private static readonly TimeSpan ProfileWindow = TimeSpan.FromDays(90);

    public async Task<PersonalizedRecommendationsDto> GetPersonalizedAsync(
        Guid userId,
        string? city,
        int maxRestaurants = 10,
        int maxItems = 10,
        CancellationToken ct = default)
    {
        var cutoff = DateTimeOffset.UtcNow.Add(-ProfileWindow);

        // Check if cold-start user
        var orderCount = await db.Orders
            .AsNoTracking()
            .CountAsync(o => o.UserId == userId && o.Status == OrderStatus.Delivered, ct);

        if (orderCount < ColdStartThreshold)
        {
            return await GetColdStartRecommendationsAsync(city, maxRestaurants, maxItems, ct);
        }

        // 1. Build user profile
        var orderItems = await db.OrderItems
            .AsNoTracking()
            .Include(oi => oi.MenuItem)
                .ThenInclude(mi => mi.Restaurant)
            .Where(oi => oi.Order.UserId == userId
                         && oi.Order.Status == OrderStatus.Delivered
                         && oi.CreatedAt >= cutoff)
            .Select(oi => new
            {
                oi.MenuItemId,
                oi.MenuItem.RestaurantId,
                oi.MenuItem.Price,
                oi.MenuItem.IsVeg,
                RestaurantCuisines = oi.MenuItem.Restaurant.CuisineTypes,
            })
            .ToListAsync(ct);

        var favoriteRestaurantIds = await db.UserFavorites
            .AsNoTracking()
            .Where(f => f.UserId == userId)
            .Select(f => f.RestaurantId)
            .ToListAsync(ct);

        var favoriteItemIds = await db.UserFavoriteItems
            .AsNoTracking()
            .Where(f => f.UserId == userId)
            .Select(f => f.MenuItemId)
            .ToListAsync(ct);

        var dietaryProfile = await db.UserDietaryProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.UserId == userId, ct);

        // Compute user preferences
        var cuisineCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var oi in orderItems)
        {
            if (oi.RestaurantCuisines is not null)
            {
                foreach (var c in oi.RestaurantCuisines.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                {
                    cuisineCounts[c] = cuisineCounts.GetValueOrDefault(c) + 1;
                }
            }
        }

        var topCuisines = cuisineCounts
            .OrderByDescending(kv => kv.Value)
            .Take(5)
            .Select(kv => kv.Key)
            .ToList();

        var avgItemPrice = orderItems.Count > 0
            ? orderItems.Average(oi => (double)oi.Price)
            : 0;

        var vegRatio = orderItems.Count > 0
            ? (double)orderItems.Count(oi => oi.IsVeg) / orderItems.Count
            : 0.5;

        var recentRestaurantIds = orderItems
            .Select(oi => oi.RestaurantId)
            .Distinct()
            .ToHashSet();

        // Recently ordered from in last 24h — exclude from recommendations
        var last24h = DateTimeOffset.UtcNow.AddHours(-24);
        var recentlyOrderedRestaurantIds = await db.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId
                        && o.Status == OrderStatus.Delivered
                        && o.CreatedAt >= last24h)
            .Select(o => o.RestaurantId)
            .Distinct()
            .ToListAsync(ct);
        var excludeRestaurantIds = recentlyOrderedRestaurantIds.ToHashSet();

        // 2. Score restaurants
        var restaurantsQuery = db.Restaurants
            .AsNoTracking()
            .Include(r => r.RestaurantCuisines)
                .ThenInclude(rc => rc.CuisineType)
            .Where(r => r.Status == RestaurantStatus.Approved
                        && r.IsAcceptingOrders
                        && !excludeRestaurantIds.Contains(r.Id));

        if (city is not null)
        {
            restaurantsQuery = restaurantsQuery.Where(r => r.City != null && EF.Functions.ILike(r.City, city));
        }

        var restaurants = await restaurantsQuery.ToListAsync(ct);

        var hasActivePromotion = await db.RestaurantPromotions
            .AsNoTracking()
            .Where(p => p.IsActive && p.ValidFrom <= DateTimeOffset.UtcNow && p.ValidUntil >= DateTimeOffset.UtcNow)
            .Select(p => p.RestaurantId)
            .Distinct()
            .ToListAsync(ct);
        var activePromotionSet = hasActivePromotion.ToHashSet();

        var scoredRestaurants = new List<(RecommendedRestaurantDto Dto, double Score)>();

        foreach (var r in restaurants)
        {
            var restaurantCuisines = r.RestaurantCuisines
                .Select(rc => rc.CuisineType.Name)
                .ToList();

            // Cuisine match (0–30)
            var cuisineOverlap = topCuisines.Count > 0
                ? (double)restaurantCuisines.Count(c => topCuisines.Contains(c, StringComparer.OrdinalIgnoreCase))
                  / topCuisines.Count * 30
                : 0;

            // Rating (0–15)
            var ratingScore = (double)r.AverageRating / 5.0 * 15;

            // Popularity (0–10)
            var popularityScore = Math.Min(r.TotalRatings / 100.0, 1.0) * 10;

            // Price match (0–10)
            var avgCost = r.AvgCostForTwo ?? 0;
            var priceMatchScore = avgItemPrice > 0 && avgCost > 0
                ? Math.Max(0, 10 - Math.Abs(avgCost - avgItemPrice * 2) / (avgItemPrice * 2) * 10)
                : 5;

            // Dietary match (0–10)
            var dietaryMatchScore = 0.0;
            if (vegRatio > 0.7 && r.IsVegOnly)
                dietaryMatchScore = 10;
            else if (vegRatio > 0.5)
                dietaryMatchScore = r.IsVegOnly ? 7 : 5;
            else
                dietaryMatchScore = 5;

            // Delivery time (0–5)
            var deliveryTimeScore = r.AvgDeliveryTimeMin.HasValue
                ? Math.Max(0, 5 - (r.AvgDeliveryTimeMin.Value - 20) / 10.0)
                : 2.5;
            deliveryTimeScore = Math.Clamp(deliveryTimeScore, 0, 5);

            // Recency boost (0–10)
            var recencyBoost = 0.0;
            if (favoriteRestaurantIds.Contains(r.Id))
                recencyBoost = 10;
            else if (recentRestaurantIds.Contains(r.Id))
                recencyBoost = 5;

            // Promotion boost (0–5)
            var promotionBoost = activePromotionSet.Contains(r.Id) ? 5.0 : 0;

            var totalScore = cuisineOverlap + ratingScore + popularityScore
                             + priceMatchScore + dietaryMatchScore
                             + deliveryTimeScore + recencyBoost + promotionBoost;

            // Normalize to 0–100
            var normalizedScore = Math.Clamp(totalScore / 95.0 * 100, 0, 100);

            // Determine recommendation reason
            var reason = DetermineRestaurantReason(
                cuisineOverlap, ratingScore, popularityScore, priceMatchScore,
                deliveryTimeScore, promotionBoost, topCuisines, restaurantCuisines);

            var dto = new RecommendedRestaurantDto(
                r.Id, r.Name, r.Slug, r.LogoUrl, r.BannerUrl, r.City,
                r.AverageRating, r.TotalRatings, r.AvgDeliveryTimeMin,
                r.AvgCostForTwo, r.IsVegOnly, r.IsAcceptingOrders,
                r.IsDineInEnabled, restaurantCuisines, reason,
                Math.Round(normalizedScore, 1));

            scoredRestaurants.Add((dto, normalizedScore));
        }

        var topRestaurants = scoredRestaurants
            .OrderByDescending(x => x.Score)
            .Take(maxRestaurants)
            .Select(x => x.Dto)
            .ToList();

        // 3. Score menu items via co-ordering
        var userOrderedItemIds = orderItems
            .Select(oi => oi.MenuItemId)
            .Distinct()
            .ToList();

        // Find similar users (share ≥2 common items)
        var similarUserIds = await db.OrderItems
            .AsNoTracking()
            .Where(oi => userOrderedItemIds.Contains(oi.MenuItemId)
                         && oi.Order.UserId != userId
                         && oi.Order.Status == OrderStatus.Delivered
                         && oi.CreatedAt >= cutoff)
            .GroupBy(oi => oi.Order.UserId)
            .Where(g => g.Select(x => x.MenuItemId).Distinct().Count() >= 2)
            .OrderByDescending(g => g.Count())
            .Take(50)
            .Select(g => g.Key)
            .ToListAsync(ct);

        // Get their other ordered items
        var coOrderedItems = await db.OrderItems
            .AsNoTracking()
            .Include(oi => oi.MenuItem)
                .ThenInclude(mi => mi.Restaurant)
            .Include(oi => oi.MenuItem)
                .ThenInclude(mi => mi.Category)
            .Where(oi => similarUserIds.Contains(oi.Order.UserId)
                         && !userOrderedItemIds.Contains(oi.MenuItemId)
                         && oi.MenuItem.IsAvailable
                         && oi.MenuItem.Restaurant.Status == RestaurantStatus.Approved
                         && oi.MenuItem.Restaurant.IsAcceptingOrders
                         && oi.Order.Status == OrderStatus.Delivered)
            .GroupBy(oi => oi.MenuItemId)
            .Select(g => new
            {
                MenuItemId = g.Key,
                CoOrderCount = g.Count(),
                Item = g.First().MenuItem,
            })
            .OrderByDescending(x => x.CoOrderCount)
            .Take(maxItems * 3)
            .ToListAsync(ct);

        var mealKeywords = GetCurrentMealKeywords();
        var scoredItems = new List<(RecommendedMenuItemDto Dto, double Score)>();

        foreach (var co in coOrderedItems)
        {
            var item = co.Item;

            // Co-order score (0–25)
            var coOrderScore = Math.Min(co.CoOrderCount / 5.0, 1.0) * 25;

            // Cuisine match (0–15)
            var itemCuisines = item.Restaurant.CuisineTypes?
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries) ?? [];
            var cuisineMatch = topCuisines.Count > 0
                ? (double)itemCuisines.Count(c => topCuisines.Contains(c, StringComparer.OrdinalIgnoreCase))
                  / topCuisines.Count * 15
                : 0;

            // Price match (0–10)
            var priceDiff = avgItemPrice > 0
                ? Math.Abs(item.Price - avgItemPrice) / avgItemPrice
                : 0;
            var itemPriceScore = Math.Max(0, 10 - priceDiff * 10);

            // Bestseller (0–10)
            var bestsellerBoost = item.IsBestseller ? 10.0 : 0;

            // Dietary match (0–10)
            var itemDietaryScore = 5.0;
            if (dietaryProfile?.AllergenAlerts is not null && item.Allergens is not null)
            {
                var hasConflict = item.Allergens.Any(a => dietaryProfile.AllergenAlerts.Contains(a));
                if (hasConflict)
                    continue; // Skip items with allergen conflicts
            }
            if (vegRatio > 0.7 && item.IsVeg)
                itemDietaryScore = 10;

            // Rating (0–10)
            var restaurantRating = (double)item.Restaurant.AverageRating / 5.0 * 10;

            // Recency (0–10)
            var itemRecencyBoost = recentRestaurantIds.Contains(item.RestaurantId) ? 7.0 : 0;
            if (favoriteItemIds.Contains(item.Id))
                itemRecencyBoost = 10;

            // Time of day (0–10)
            var timeBoost = 0.0;
            var categoryName = item.Category?.Name?.ToLowerInvariant() ?? "";
            var itemName = item.Name.ToLowerInvariant();
            if (mealKeywords.Any(k => categoryName.Contains(k) || itemName.Contains(k)))
                timeBoost = 10;

            var totalItemScore = coOrderScore + cuisineMatch + itemPriceScore
                                 + bestsellerBoost + itemDietaryScore
                                 + restaurantRating + itemRecencyBoost + timeBoost;

            var normalizedItemScore = Math.Clamp(totalItemScore / 100.0 * 100, 0, 100);

            var reason = DetermineItemReason(
                coOrderScore, cuisineMatch, bestsellerBoost, timeBoost,
                topCuisines, itemCuisines);

            var dto = new RecommendedMenuItemDto(
                item.Id, item.Name, item.Description,
                item.Price, item.DiscountedPrice, item.ImageUrl,
                item.IsVeg, item.IsBestseller,
                item.RestaurantId, item.Restaurant.Name, item.Restaurant.Slug,
                reason, Math.Round(normalizedItemScore, 1));

            scoredItems.Add((dto, normalizedItemScore));
        }

        var topItems = scoredItems
            .OrderByDescending(x => x.Score)
            .Take(maxItems)
            .Select(x => x.Dto)
            .ToList();

        return new PersonalizedRecommendationsDto(topRestaurants, topItems);
    }

    public async Task<List<TrendingItemDto>> GetTrendingAsync(
        string? city,
        int count = 20,
        CancellationToken ct = default)
    {
        var since = DateTimeOffset.UtcNow.AddHours(-24);

        var query = db.OrderItems
            .AsNoTracking()
            .Include(oi => oi.MenuItem)
                .ThenInclude(mi => mi.Restaurant)
            .Where(oi => oi.CreatedAt >= since
                         && oi.MenuItem.IsAvailable
                         && oi.MenuItem.Restaurant.Status == RestaurantStatus.Approved
                         && oi.MenuItem.Restaurant.IsAcceptingOrders);

        if (city is not null)
        {
            query = query.Where(oi => oi.MenuItem.Restaurant.City != null
                                      && EF.Functions.ILike(oi.MenuItem.Restaurant.City, city));
        }

        var trending = await query
            .GroupBy(oi => oi.MenuItemId)
            .Select(g => new
            {
                MenuItemId = g.Key,
                OrderCount = g.Sum(x => x.Quantity),
            })
            .OrderByDescending(x => x.OrderCount)
            .Take(count)
            .ToListAsync(ct);

        var itemIds = trending.Select(t => t.MenuItemId).ToList();
        var items = await db.MenuItems
            .AsNoTracking()
            .Include(mi => mi.Restaurant)
            .Where(mi => itemIds.Contains(mi.Id))
            .ToDictionaryAsync(mi => mi.Id, ct);

        var result = new List<TrendingItemDto>();
        var rank = 1;
        foreach (var t in trending)
        {
            if (!items.TryGetValue(t.MenuItemId, out var item))
                continue;

            result.Add(new TrendingItemDto(
                item.Id, item.Name, item.ImageUrl,
                item.Price, item.IsVeg, item.IsBestseller,
                item.RestaurantId, item.Restaurant.Name,
                t.OrderCount, rank++));
        }

        return result;
    }

    public async Task<List<RecommendedRestaurantDto>> GetSimilarRestaurantsAsync(
        Guid restaurantId,
        int count = 10,
        CancellationToken ct = default)
    {
        var source = await db.Restaurants
            .AsNoTracking()
            .Include(r => r.RestaurantCuisines)
                .ThenInclude(rc => rc.CuisineType)
            .FirstOrDefaultAsync(r => r.Id == restaurantId, ct);

        if (source is null)
            return [];

        var sourceCuisines = source.RestaurantCuisines
            .Select(rc => rc.CuisineType.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var candidates = await db.Restaurants
            .AsNoTracking()
            .Include(r => r.RestaurantCuisines)
                .ThenInclude(rc => rc.CuisineType)
            .Where(r => r.Id != restaurantId
                        && r.Status == RestaurantStatus.Approved
                        && r.IsAcceptingOrders
                        && r.City != null
                        && source.City != null
                        && EF.Functions.ILike(r.City, source.City))
            .ToListAsync(ct);

        var scored = new List<(RecommendedRestaurantDto Dto, double Score)>();

        foreach (var r in candidates)
        {
            var candidateCuisines = r.RestaurantCuisines
                .Select(rc => rc.CuisineType.Name)
                .ToList();

            var candidateSet = candidateCuisines.ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Jaccard similarity for cuisines (0–40)
            var intersection = sourceCuisines.Count(c => candidateSet.Contains(c));
            var union = sourceCuisines.Count + candidateSet.Count - intersection;
            var cuisineScore = union > 0 ? (double)intersection / union * 40 : 0;

            // Rating (0–30)
            var ratingScore = (double)r.AverageRating / 5.0 * 30;

            // Popularity (0–20)
            var popularityScore = Math.Min(r.TotalRatings / 100.0, 1.0) * 20;

            // Delivery time (0–10)
            var deliveryScore = r.AvgDeliveryTimeMin.HasValue
                ? Math.Max(0, 10 - (r.AvgDeliveryTimeMin.Value - 20) / 10.0)
                : 5;
            deliveryScore = Math.Clamp(deliveryScore, 0, 10);

            var totalScore = cuisineScore + ratingScore + popularityScore + deliveryScore;
            var normalized = Math.Clamp(totalScore, 0, 100);

            string? reason = cuisineScore >= ratingScore
                ? "Similar cuisine"
                : "Highly rated";

            scored.Add((new RecommendedRestaurantDto(
                r.Id, r.Name, r.Slug, r.LogoUrl, r.BannerUrl, r.City,
                r.AverageRating, r.TotalRatings, r.AvgDeliveryTimeMin,
                r.AvgCostForTwo, r.IsVegOnly, r.IsAcceptingOrders,
                r.IsDineInEnabled, candidateCuisines, reason,
                Math.Round(normalized, 1)), normalized));
        }

        return scored
            .OrderByDescending(x => x.Score)
            .Take(count)
            .Select(x => x.Dto)
            .ToList();
    }

    public async Task<List<RecommendedMenuItemDto>> GetSimilarItemsAsync(
        Guid menuItemId,
        int count = 10,
        CancellationToken ct = default)
    {
        var cutoff = DateTimeOffset.UtcNow.Add(-ProfileWindow);

        // Find users who ordered this item
        var usersWhoOrdered = await db.OrderItems
            .AsNoTracking()
            .Where(oi => oi.MenuItemId == menuItemId
                         && oi.Order.Status == OrderStatus.Delivered
                         && oi.CreatedAt >= cutoff)
            .Select(oi => oi.Order.UserId)
            .Distinct()
            .Take(100)
            .ToListAsync(ct);

        if (usersWhoOrdered.Count == 0)
            return [];

        // Find their other ordered items ranked by co-occurrence
        var coOrdered = await db.OrderItems
            .AsNoTracking()
            .Include(oi => oi.MenuItem)
                .ThenInclude(mi => mi.Restaurant)
            .Where(oi => usersWhoOrdered.Contains(oi.Order.UserId)
                         && oi.MenuItemId != menuItemId
                         && oi.MenuItem.IsAvailable
                         && oi.MenuItem.Restaurant.Status == RestaurantStatus.Approved
                         && oi.MenuItem.Restaurant.IsAcceptingOrders
                         && oi.Order.Status == OrderStatus.Delivered)
            .GroupBy(oi => oi.MenuItemId)
            .Select(g => new
            {
                MenuItemId = g.Key,
                Count = g.Count(),
                Item = g.First().MenuItem,
            })
            .OrderByDescending(x => x.Count)
            .Take(count)
            .ToListAsync(ct);

        return coOrdered.Select((co, idx) =>
        {
            var item = co.Item;
            var score = Math.Clamp((double)co.Count / usersWhoOrdered.Count * 100, 0, 100);

            return new RecommendedMenuItemDto(
                item.Id, item.Name, item.Description,
                item.Price, item.DiscountedPrice, item.ImageUrl,
                item.IsVeg, item.IsBestseller,
                item.RestaurantId, item.Restaurant.Name, item.Restaurant.Slug,
                "Customers also ordered",
                Math.Round(score, 1));
        }).ToList();
    }

    private async Task<PersonalizedRecommendationsDto> GetColdStartRecommendationsAsync(
        string? city,
        int maxRestaurants,
        int maxItems,
        CancellationToken ct)
    {
        var restaurantsQuery = db.Restaurants
            .AsNoTracking()
            .Include(r => r.RestaurantCuisines)
                .ThenInclude(rc => rc.CuisineType)
            .Where(r => r.Status == RestaurantStatus.Approved && r.IsAcceptingOrders);

        if (city is not null)
        {
            restaurantsQuery = restaurantsQuery.Where(r => r.City != null && EF.Functions.ILike(r.City, city));
        }

        var topRatedRestaurants = await restaurantsQuery
            .OrderByDescending(r => r.AverageRating)
            .ThenByDescending(r => r.TotalRatings)
            .Take(maxRestaurants)
            .ToListAsync(ct);

        var restaurantDtos = topRatedRestaurants.Select(r => new RecommendedRestaurantDto(
            r.Id, r.Name, r.Slug, r.LogoUrl, r.BannerUrl, r.City,
            r.AverageRating, r.TotalRatings, r.AvgDeliveryTimeMin,
            r.AvgCostForTwo, r.IsVegOnly, r.IsAcceptingOrders,
            r.IsDineInEnabled,
            r.RestaurantCuisines.Select(rc => rc.CuisineType.Name).ToList(),
            "Popular in your area",
            Math.Round((double)r.AverageRating / 5.0 * 100, 1))).ToList();

        var topRestaurantIds = topRatedRestaurants.Select(r => r.Id).ToList();
        var bestsellerItems = await db.MenuItems
            .AsNoTracking()
            .Include(mi => mi.Restaurant)
            .Where(mi => mi.IsBestseller
                         && mi.IsAvailable
                         && topRestaurantIds.Contains(mi.RestaurantId))
            .OrderByDescending(mi => mi.Restaurant.AverageRating)
            .Take(maxItems)
            .ToListAsync(ct);

        var itemDtos = bestsellerItems.Select(item => new RecommendedMenuItemDto(
            item.Id, item.Name, item.Description,
            item.Price, item.DiscountedPrice, item.ImageUrl,
            item.IsVeg, item.IsBestseller,
            item.RestaurantId, item.Restaurant.Name, item.Restaurant.Slug,
            "Bestseller",
            Math.Round((double)item.Restaurant.AverageRating / 5.0 * 100, 1))).ToList();

        return new PersonalizedRecommendationsDto(restaurantDtos, itemDtos);
    }

    private static string? DetermineRestaurantReason(
        double cuisineScore, double ratingScore, double popularityScore,
        double priceScore, double deliveryScore, double promotionBoost,
        List<string> topCuisines, List<string> restaurantCuisines)
    {
        var maxScore = Math.Max(cuisineScore, Math.Max(ratingScore,
            Math.Max(popularityScore, Math.Max(deliveryScore, promotionBoost))));

        if (maxScore == promotionBoost && promotionBoost > 0)
            return "Great deal available";
        if (maxScore == deliveryScore && deliveryScore >= 4)
            return "Quick delivery";
        if (maxScore == cuisineScore && topCuisines.Count > 0)
        {
            var matchedCuisine = restaurantCuisines
                .FirstOrDefault(c => topCuisines.Contains(c, StringComparer.OrdinalIgnoreCase));
            return matchedCuisine is not null ? $"Because you like {matchedCuisine}" : "Popular in your area";
        }
        if (maxScore == popularityScore)
            return "Popular in your area";
        return "Highly rated";
    }

    private static string? DetermineItemReason(
        double coOrderScore, double cuisineScore, double bestsellerBoost,
        double timeBoost, List<string> topCuisines, string[] itemCuisines)
    {
        var maxScore = Math.Max(coOrderScore, Math.Max(cuisineScore,
            Math.Max(bestsellerBoost, timeBoost)));

        if (maxScore == coOrderScore && coOrderScore > 0)
            return "Customers also ordered";
        if (maxScore == timeBoost && timeBoost > 0)
            return "Perfect for this time";
        if (maxScore == bestsellerBoost && bestsellerBoost > 0)
            return "Bestseller";
        if (maxScore == cuisineScore && topCuisines.Count > 0)
        {
            var matched = itemCuisines
                .FirstOrDefault(c => topCuisines.Contains(c, StringComparer.OrdinalIgnoreCase));
            return matched is not null ? $"Because you like {matched}" : null;
        }
        return null;
    }

    private static string[] GetCurrentMealKeywords()
    {
        var hour = DateTime.UtcNow.Hour;
        return hour switch
        {
            >= 5 and < 11 => BreakfastKeywords,
            >= 11 and < 15 => LunchKeywords,
            >= 15 and < 18 => SnackKeywords,
            >= 18 and < 23 => DinnerKeywords,
            _ => SnackKeywords,
        };
    }
}
