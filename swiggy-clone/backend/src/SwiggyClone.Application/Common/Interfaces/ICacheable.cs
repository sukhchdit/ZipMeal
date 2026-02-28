namespace SwiggyClone.Application.Common.Interfaces;

/// <summary>
/// Marker interface for MediatR requests whose responses should be cached.
/// Implement this on query requests to enable automatic distributed caching
/// through <see cref="Behaviors.CachingBehavior{TRequest, TResponse}"/>.
/// </summary>
public interface ICacheable
{
    /// <summary>
    /// A unique key used to store and retrieve the cached response.
    /// </summary>
    string CacheKey { get; }

    /// <summary>
    /// The duration in minutes for which the cached response remains valid.
    /// </summary>
    int CacheExpirationMinutes { get; }
}
