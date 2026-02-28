using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;

namespace SwiggyClone.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that provides idempotency protection
/// for requests implementing <see cref="IIdempotent"/>. Duplicate requests
/// with the same idempotency key return the cached response from the first execution.
/// Requests that do not implement <see cref="IIdempotent"/> pass through unaffected.
/// </summary>
public sealed class IdempotencyBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(60);

    private readonly IDistributedCache _cache;
    private readonly ILogger<IdempotencyBehavior<TRequest, TResponse>> _logger;

    public IdempotencyBehavior(
        IDistributedCache cache,
        ILogger<IdempotencyBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IIdempotent idempotent || string.IsNullOrWhiteSpace(idempotent.IdempotencyKey))
        {
            return await next();
        }

        var cacheKey = $"idempotency:{typeof(TRequest).Name}:{idempotent.IdempotencyKey}";

        try
        {
            var cachedBytes = await _cache.GetAsync(cacheKey, cancellationToken);

            if (cachedBytes is not null)
            {
                _logger.LogDebug("Idempotency hit for key {IdempotencyKey}", cacheKey);

                var cachedResponse = JsonSerializer.Deserialize<TResponse>(cachedBytes, JsonOptions);

                if (cachedResponse is not null)
                {
                    return cachedResponse;
                }

                _logger.LogWarning(
                    "Idempotency deserialization returned null for key {IdempotencyKey}; executing handler",
                    cacheKey);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Idempotency cache read failed for key {IdempotencyKey}; executing handler", cacheKey);
        }

        _logger.LogDebug("Idempotency miss for key {IdempotencyKey}; executing handler", cacheKey);

        var response = await next();

        try
        {
            var serializedBytes = JsonSerializer.SerializeToUtf8Bytes(response, JsonOptions);
            await _cache.SetAsync(cacheKey, serializedBytes, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = DefaultExpiration
            }, cancellationToken);

            _logger.LogDebug("Cached idempotency response for key {IdempotencyKey}", cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Failed to cache idempotency response for key {IdempotencyKey}; response will be returned uncached",
                cacheKey);
        }

        return response;
    }
}
