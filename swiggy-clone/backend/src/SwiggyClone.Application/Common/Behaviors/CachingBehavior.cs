using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;

namespace SwiggyClone.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that provides transparent distributed caching
/// for requests implementing <see cref="ICacheable"/>. Cached responses are
/// stored and retrieved via <see cref="IDistributedCache"/>.
/// Requests that do not implement <see cref="ICacheable"/> pass through unaffected.
/// </summary>
/// <typeparam name="TRequest">The type of the MediatR request.</typeparam>
/// <typeparam name="TResponse">The type of the MediatR response.</typeparam>
public sealed class CachingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly IDistributedCache _cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(
        IDistributedCache cache,
        ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ICacheable cacheable)
        {
            return await next();
        }

        var cacheKey = cacheable.CacheKey;

        var cachedBytes = await _cache.GetAsync(cacheKey, cancellationToken);

        if (cachedBytes is not null)
        {
            _logger.LogDebug("Cache hit for key {CacheKey}", cacheKey);

            var cachedResponse = JsonSerializer.Deserialize<TResponse>(cachedBytes, JsonOptions);

            if (cachedResponse is not null)
            {
                return cachedResponse;
            }

            _logger.LogWarning(
                "Cache deserialization returned null for key {CacheKey}; executing handler",
                cacheKey);
        }

        _logger.LogDebug("Cache miss for key {CacheKey}; executing handler", cacheKey);

        var response = await next();

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheable.CacheExpirationMinutes)
        };

        try
        {
            var serializedBytes = JsonSerializer.SerializeToUtf8Bytes(response, JsonOptions);
            await _cache.SetAsync(cacheKey, serializedBytes, options, cancellationToken);

            _logger.LogDebug(
                "Cached response for key {CacheKey} with expiration {ExpirationMinutes}min",
                cacheKey,
                cacheable.CacheExpirationMinutes);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to cache response for key {CacheKey}; response will be returned uncached",
                cacheKey);
        }

        return response;
    }
}
