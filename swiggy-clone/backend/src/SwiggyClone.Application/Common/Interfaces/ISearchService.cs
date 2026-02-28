using SwiggyClone.Application.Features.Discovery.Documents;
using SwiggyClone.Application.Features.Discovery.DTOs;

namespace SwiggyClone.Application.Common.Interfaces;

/// <summary>
/// Abstraction over Elasticsearch for full-text search, autocomplete,
/// and index management. Implementations live in Infrastructure.
/// </summary>
public interface ISearchService
{
    // ─────────────────────── Health ──────────────────────────────

    Task<bool> IsAvailableAsync(CancellationToken ct = default);

    // ─────────────────────── Indexing ────────────────────────────

    Task IndexRestaurantAsync(RestaurantSearchDocument document, CancellationToken ct = default);
    Task IndexMenuItemAsync(MenuItemSearchDocument document, CancellationToken ct = default);
    Task DeleteRestaurantAsync(Guid restaurantId, CancellationToken ct = default);
    Task DeleteMenuItemAsync(Guid menuItemId, CancellationToken ct = default);
    Task DeleteMenuItemsByRestaurantAsync(Guid restaurantId, CancellationToken ct = default);
    Task BulkIndexRestaurantsAsync(IEnumerable<RestaurantSearchDocument> documents, CancellationToken ct = default);
    Task BulkIndexMenuItemsAsync(IEnumerable<MenuItemSearchDocument> documents, CancellationToken ct = default);

    // ─────────────────────── Search ─────────────────────────────

    Task<List<RestaurantSearchDocument>> SearchRestaurantsAsync(
        string term, string? city, int pageSize, CancellationToken ct = default);

    Task<List<MenuItemSearchDocument>> SearchMenuItemsAsync(
        string term, string? city, int pageSize, CancellationToken ct = default);

    // ─────────────────────── Suggestions ────────────────────────

    Task<List<SearchSuggestionDto>> GetSuggestionsAsync(
        string prefix, string? city, int limit = 10, CancellationToken ct = default);

    // ─────────────────────── Index Management ───────────────────

    Task EnsureIndicesCreatedAsync(CancellationToken ct = default);
    Task RecreateIndicesAsync(CancellationToken ct = default);
}
