namespace SwiggyClone.Application.Features.Discovery.Documents;

/// <summary>
/// Elasticsearch document for the "swiggyclone-menuitems" index.
/// Denormalized from MenuItem entity + parent Restaurant metadata.
/// </summary>
public sealed class MenuItemSearchDocument
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Price { get; set; }
    public int? DiscountedPrice { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsVeg { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsBestseller { get; set; }

    // Parent restaurant info (denormalized for search results)
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public string RestaurantSlug { get; set; } = string.Empty;
    public string? RestaurantLogoUrl { get; set; }
    public string? RestaurantCity { get; set; }
    public decimal RestaurantAverageRating { get; set; }
    public int RestaurantTotalRatings { get; set; }
    public bool RestaurantIsAcceptingOrders { get; set; }
    public string NameSuggest { get; set; } = string.Empty;
}
