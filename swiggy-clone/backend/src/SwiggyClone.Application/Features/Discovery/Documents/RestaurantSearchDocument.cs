namespace SwiggyClone.Application.Features.Discovery.Documents;

/// <summary>
/// Elasticsearch document for the "swiggyclone-restaurants" index.
/// Denormalized from Restaurant entity + RestaurantCuisines.
/// </summary>
public sealed class RestaurantSearchDocument
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? BannerUrl { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalRatings { get; set; }
    public int? AvgDeliveryTimeMin { get; set; }
    public int? AvgCostForTwo { get; set; }
    public bool IsVegOnly { get; set; }
    public bool IsAcceptingOrders { get; set; }
    public bool IsDineInEnabled { get; set; }
    public List<string> Cuisines { get; set; } = [];
    public string NameSuggest { get; set; } = string.Empty;
}
