using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Discovery.Documents;
using SwiggyClone.Application.Features.Discovery.DTOs;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Infrastructure.Services;

internal sealed class ElasticsearchService(
    ElasticsearchClient client,
    ILogger<ElasticsearchService> logger,
    [FromKeyedServices("elasticsearch")] ResiliencePipeline pipeline) : ISearchService
{
    private static readonly string[] RestaurantSearchFields = ["name^3", "description", "cuisines^2"];
    private static readonly string[] MenuItemSearchFields = ["name^3", "description", "restaurantName"];
    private static readonly string[] SuggestFields = ["nameSuggest", "nameSuggest._2gram", "nameSuggest._3gram"];

    // ─────────────────────── Health ──────────────────────────────

    public async Task<bool> IsAvailableAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await client.PingAsync(ct);
            return response.IsValidResponse;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Elasticsearch ping failed");
            return false;
        }
    }

    // ─────────────────────── Single-Document Indexing ────────────

    public async Task IndexRestaurantAsync(RestaurantSearchDocument document, CancellationToken ct = default)
    {
        await pipeline.ExecuteAsync(async token =>
        {
            var response = await client.IndexAsync(document, ElasticsearchConstants.RestaurantIndex, document.Id.ToString(), token);
            if (!response.IsValidResponse)
                logger.LogWarning("Failed to index restaurant {Id}: {Info}", document.Id, response.DebugInformation);
        }, ct);
    }

    public async Task IndexMenuItemAsync(MenuItemSearchDocument document, CancellationToken ct = default)
    {
        await pipeline.ExecuteAsync(async token =>
        {
            var response = await client.IndexAsync(document, ElasticsearchConstants.MenuItemIndex, document.Id.ToString(), token);
            if (!response.IsValidResponse)
                logger.LogWarning("Failed to index menu item {Id}: {Info}", document.Id, response.DebugInformation);
        }, ct);
    }

    // ─────────────────────── Delete ─────────────────────────────

    public async Task DeleteRestaurantAsync(Guid restaurantId, CancellationToken ct = default)
    {
        await pipeline.ExecuteAsync(async token =>
        {
            var request = new DeleteRequest(ElasticsearchConstants.RestaurantIndex, restaurantId.ToString());
            await client.DeleteAsync(request, token);
        }, ct);
    }

    public async Task DeleteMenuItemAsync(Guid menuItemId, CancellationToken ct = default)
    {
        await pipeline.ExecuteAsync(async token =>
        {
            var request = new DeleteRequest(ElasticsearchConstants.MenuItemIndex, menuItemId.ToString());
            await client.DeleteAsync(request, token);
        }, ct);
    }

    public async Task DeleteMenuItemsByRestaurantAsync(Guid restaurantId, CancellationToken ct = default)
    {
        await pipeline.ExecuteAsync(async token =>
        {
            await client.DeleteByQueryAsync<MenuItemSearchDocument>(ElasticsearchConstants.MenuItemIndex, d => d
                .Query(q => q
                    .Term(t => t
                        .Field(f => f.RestaurantId)
                        .Value(restaurantId.ToString())
                    )
                ),
                token);
        }, ct);
    }

    // ─────────────────────── Bulk Indexing ───────────────────────

    public async Task BulkIndexRestaurantsAsync(
        IEnumerable<RestaurantSearchDocument> documents, CancellationToken ct = default)
    {
        await pipeline.ExecuteAsync(async token =>
        {
            var response = await client.BulkAsync(b => b
                .Index(ElasticsearchConstants.RestaurantIndex)
                .IndexMany(documents, (op, doc) => op.Id(doc.Id.ToString())),
                token);

            if (response.Errors)
                logger.LogWarning("Bulk index restaurants had errors: {Count} failed",
                    response.ItemsWithErrors.Count());
        }, ct);
    }

    public async Task BulkIndexMenuItemsAsync(
        IEnumerable<MenuItemSearchDocument> documents, CancellationToken ct = default)
    {
        await pipeline.ExecuteAsync(async token =>
        {
            var response = await client.BulkAsync(b => b
                .Index(ElasticsearchConstants.MenuItemIndex)
                .IndexMany(documents, (op, doc) => op.Id(doc.Id.ToString())),
                token);

            if (response.Errors)
                logger.LogWarning("Bulk index menu items had errors: {Count} failed",
                    response.ItemsWithErrors.Count());
        }, ct);
    }

    // ─────────────────────── Search ─────────────────────────────

    public async Task<List<RestaurantSearchDocument>> SearchRestaurantsAsync(
        string term, string? city, int pageSize, CancellationToken ct = default)
    {
        try
        {
            return await pipeline.ExecuteAsync(async token =>
            {
                var response = await client.SearchAsync<RestaurantSearchDocument>(s => s
                    .Index(ElasticsearchConstants.RestaurantIndex)
                    .Size(pageSize)
                    .Query(q => q
                        .Bool(b =>
                        {
                            b.Must(m => m
                                .MultiMatch(mm => mm
                                    .Query(term)
                                    .Fields(RestaurantSearchFields)
                                    .Type(TextQueryType.BestFields)
                                    .Fuzziness(new Fuzziness("AUTO"))
                                )
                            );

                            if (!string.IsNullOrWhiteSpace(city))
                            {
                                b.Filter(f => f
                                    .Term(t => t.Field(new Field("city")).Value(city.ToLowerInvariant()))
                                );
                            }
                        })
                    )
                    .Sort(so => so
                        .Score(new ScoreSort { Order = SortOrder.Desc })
                        .Field(new Field("averageRating"), new FieldSort { Order = SortOrder.Desc })
                        .Field(new Field("totalRatings"), new FieldSort { Order = SortOrder.Desc })
                    ),
                    token);

                if (!response.IsValidResponse)
                {
                    logger.LogWarning("ES restaurant search failed: {Info}", response.DebugInformation);
                    return [];
                }

                return response.Documents.ToList();
            }, ct);
        }
        catch (BrokenCircuitException)
        {
            logger.LogWarning("Elasticsearch circuit open — returning empty restaurant results");
            return [];
        }
        catch (TimeoutRejectedException)
        {
            logger.LogWarning("Elasticsearch search timed out — returning empty restaurant results");
            return [];
        }
    }

    public async Task<List<MenuItemSearchDocument>> SearchMenuItemsAsync(
        string term, string? city, int pageSize, CancellationToken ct = default)
    {
        try
        {
            return await pipeline.ExecuteAsync(async token =>
            {
                var response = await client.SearchAsync<MenuItemSearchDocument>(s => s
                    .Index(ElasticsearchConstants.MenuItemIndex)
                    .Size(pageSize)
                    .Query(q => q
                        .Bool(b =>
                        {
                            b.Must(m => m
                                .MultiMatch(mm => mm
                                    .Query(term)
                                    .Fields(MenuItemSearchFields)
                                    .Type(TextQueryType.BestFields)
                                    .Fuzziness(new Fuzziness("AUTO"))
                                )
                            );

                            b.Filter(f => f
                                .Term(t => t.Field(new Field("isAvailable")).Value(true))
                            );

                            if (!string.IsNullOrWhiteSpace(city))
                            {
                                b.Filter(f => f
                                    .Term(t => t.Field(new Field("restaurantCity")).Value(city.ToLowerInvariant()))
                                );
                            }
                        })
                    )
                    .Sort(so => so
                        .Score(new ScoreSort { Order = SortOrder.Desc })
                    ),
                    token);

                if (!response.IsValidResponse)
                {
                    logger.LogWarning("ES menu item search failed: {Info}", response.DebugInformation);
                    return [];
                }

                return response.Documents.ToList();
            }, ct);
        }
        catch (BrokenCircuitException)
        {
            logger.LogWarning("Elasticsearch circuit open — returning empty menu item results");
            return [];
        }
        catch (TimeoutRejectedException)
        {
            logger.LogWarning("Elasticsearch search timed out — returning empty menu item results");
            return [];
        }
    }

    // ─────────────────────── Suggestions ────────────────────────

    public async Task<List<SearchSuggestionDto>> GetSuggestionsAsync(
        string prefix, string? city, int limit = 10, CancellationToken ct = default)
    {
        try
        {
            return await pipeline.ExecuteAsync(async token =>
            {
                var halfLimit = limit / 2 + 1;

                // Search restaurants
                var restaurantTask = client.SearchAsync<RestaurantSearchDocument>(s => s
                    .Index(ElasticsearchConstants.RestaurantIndex)
                    .Size(halfLimit)
                    .Query(q => q.Bool(b =>
                    {
                        b.Must(m => m.MultiMatch(mm => mm
                            .Query(prefix)
                            .Type(TextQueryType.BoolPrefix)
                            .Fields(SuggestFields)
                        ));
                        if (!string.IsNullOrWhiteSpace(city))
                            b.Filter(f => f.Term(t => t.Field(new Field("city")).Value(city.ToLowerInvariant())));
                    })),
                    token);

                // Search menu items
                var menuItemTask = client.SearchAsync<MenuItemSearchDocument>(s => s
                    .Index(ElasticsearchConstants.MenuItemIndex)
                    .Size(halfLimit)
                    .Query(q => q.Bool(b =>
                    {
                        b.Must(m => m.MultiMatch(mm => mm
                            .Query(prefix)
                            .Type(TextQueryType.BoolPrefix)
                            .Fields(SuggestFields)
                        ));
                        if (!string.IsNullOrWhiteSpace(city))
                            b.Filter(f => f.Term(t => t.Field(new Field("restaurantCity")).Value(city.ToLowerInvariant())));
                    })),
                    token);

                await Task.WhenAll(restaurantTask, menuItemTask);

                var suggestions = new List<SearchSuggestionDto>();

                if (restaurantTask.Result.IsValidResponse)
                {
                    suggestions.AddRange(restaurantTask.Result.Documents.Select(d =>
                        new SearchSuggestionDto(d.Name, "restaurant", d.Id, null, null, d.LogoUrl)));
                }

                if (menuItemTask.Result.IsValidResponse)
                {
                    suggestions.AddRange(menuItemTask.Result.Documents.Select(d =>
                        new SearchSuggestionDto(d.Name, "dish", d.Id, d.RestaurantId, d.RestaurantName, d.ImageUrl)));
                }

                return suggestions.Take(limit).ToList();
            }, ct);
        }
        catch (BrokenCircuitException)
        {
            logger.LogWarning("Elasticsearch circuit open — returning empty suggestions");
            return [];
        }
        catch (TimeoutRejectedException)
        {
            logger.LogWarning("Elasticsearch suggestions timed out — returning empty suggestions");
            return [];
        }
    }

    // ─────────────────────── Index Management ───────────────────

    public async Task EnsureIndicesCreatedAsync(CancellationToken ct = default)
    {
        await EnsureRestaurantIndexAsync(ct);
        await EnsureMenuItemIndexAsync(ct);
    }

    public async Task RecreateIndicesAsync(CancellationToken ct = default)
    {
        // Delete if exists
        var restaurantExists = await client.Indices.ExistsAsync(ElasticsearchConstants.RestaurantIndex, ct);
        if (restaurantExists.Exists)
            await client.Indices.DeleteAsync(ElasticsearchConstants.RestaurantIndex, ct);

        var menuItemExists = await client.Indices.ExistsAsync(ElasticsearchConstants.MenuItemIndex, ct);
        if (menuItemExists.Exists)
            await client.Indices.DeleteAsync(ElasticsearchConstants.MenuItemIndex, ct);

        await EnsureIndicesCreatedAsync(ct);
    }

    private async Task EnsureRestaurantIndexAsync(CancellationToken ct)
    {
        var exists = await client.Indices.ExistsAsync(ElasticsearchConstants.RestaurantIndex, ct);
        if (exists.Exists) return;

        var response = await client.Indices.CreateAsync(ElasticsearchConstants.RestaurantIndex, c => c
            .Settings(s => s
                .NumberOfShards(1)
                .NumberOfReplicas(0)
            )
            .Mappings(m => m
                .Properties<RestaurantSearchDocument>(p => p
                    .Keyword(k => k.Id)
                    .Text(t => t.Name, t => t.Fields(f => f
                        .Keyword(k => k.Name.Suffix("keyword"))
                    ))
                    .SearchAsYouType(t => t.NameSuggest)
                    .Keyword(k => k.Slug)
                    .Text(t => t.Description!)
                    .Keyword(k => k.LogoUrl!, k => k.Index(false))
                    .Keyword(k => k.BannerUrl!, k => k.Index(false))
                    .Keyword(k => k.City!)
                    .Keyword(k => k.State!)
                    .FloatNumber(n => n.AverageRating)
                    .IntegerNumber(n => n.TotalRatings)
                    .IntegerNumber(n => n.AvgDeliveryTimeMin!)
                    .IntegerNumber(n => n.AvgCostForTwo!)
                    .Boolean(b => b.IsVegOnly)
                    .Boolean(b => b.IsAcceptingOrders)
                    .Boolean(b => b.IsDineInEnabled)
                    .Text(t => t.Cuisines, t => t.Fields(f => f
                        .Keyword(k => k.Cuisines.Suffix("keyword"))
                    ))
                )
            ),
            ct);

        if (!response.IsValidResponse)
            logger.LogError("Failed to create restaurant index: {Info}", response.DebugInformation);
        else
            logger.LogInformation("Created Elasticsearch index: {Index}", ElasticsearchConstants.RestaurantIndex);
    }

    private async Task EnsureMenuItemIndexAsync(CancellationToken ct)
    {
        var exists = await client.Indices.ExistsAsync(ElasticsearchConstants.MenuItemIndex, ct);
        if (exists.Exists) return;

        var response = await client.Indices.CreateAsync(ElasticsearchConstants.MenuItemIndex, c => c
            .Settings(s => s
                .NumberOfShards(1)
                .NumberOfReplicas(0)
            )
            .Mappings(m => m
                .Properties<MenuItemSearchDocument>(p => p
                    .Keyword(k => k.Id)
                    .Text(t => t.Name, t => t.Fields(f => f
                        .Keyword(k => k.Name.Suffix("keyword"))
                    ))
                    .SearchAsYouType(t => t.NameSuggest)
                    .Text(t => t.Description!)
                    .IntegerNumber(n => n.Price)
                    .IntegerNumber(n => n.DiscountedPrice!)
                    .Keyword(k => k.ImageUrl!, k => k.Index(false))
                    .Boolean(b => b.IsVeg)
                    .Boolean(b => b.IsAvailable)
                    .Boolean(b => b.IsBestseller)
                    .Keyword(k => k.RestaurantId)
                    .Text(t => t.RestaurantName, t => t.Fields(f => f
                        .Keyword(k => k.RestaurantName.Suffix("keyword"))
                    ))
                    .Keyword(k => k.RestaurantSlug, k => k.Index(false))
                    .Keyword(k => k.RestaurantLogoUrl!, k => k.Index(false))
                    .Keyword(k => k.RestaurantCity!)
                    .FloatNumber(n => n.RestaurantAverageRating)
                    .IntegerNumber(n => n.RestaurantTotalRatings)
                    .Boolean(b => b.RestaurantIsAcceptingOrders)
                )
            ),
            ct);

        if (!response.IsValidResponse)
            logger.LogError("Failed to create menu item index: {Info}", response.DebugInformation);
        else
            logger.LogInformation("Created Elasticsearch index: {Index}", ElasticsearchConstants.MenuItemIndex);
    }
}
