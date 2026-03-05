using System.Text.Json;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SwiggyClone.Application.Common.Behaviors;
using SwiggyClone.Application.Common.Interfaces;

namespace SwiggyClone.UnitTests.Behaviors;

public sealed class CachingBehaviorTests
{
    public sealed record NonCacheableRequest : IRequest<string>;

    public sealed record CacheableRequest : IRequest<string>, ICacheable
    {
        public string CacheKey => "test-key";
        public int CacheExpirationMinutes => 5;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    private readonly IDistributedCache _cache = Substitute.For<IDistributedCache>();

    private readonly ILogger<CachingBehavior<CacheableRequest, string>> _logger =
        Substitute.For<ILogger<CachingBehavior<CacheableRequest, string>>>();

    private readonly RequestHandlerDelegate<string> _next = Substitute.For<RequestHandlerDelegate<string>>();

    [Fact]
    public async Task Handle_NonCacheableRequest_BypassesCache()
    {
        var nonCacheableNext = Substitute.For<RequestHandlerDelegate<string>>();
        nonCacheableNext.Invoke().Returns("direct");
        var nonCacheableCache = Substitute.For<IDistributedCache>();
        var nonCacheableLogger =
            Substitute.For<ILogger<CachingBehavior<NonCacheableRequest, string>>>();

        var behavior = new CachingBehavior<NonCacheableRequest, string>(nonCacheableCache, nonCacheableLogger);

        var result = await behavior.Handle(new NonCacheableRequest(), nonCacheableNext, CancellationToken.None);

        result.Should().Be("direct");
        await nonCacheableCache.DidNotReceive().GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CacheHit_ReturnsCachedValue()
    {
        var cachedBytes = JsonSerializer.SerializeToUtf8Bytes("cached-value", JsonOptions);
        _cache.GetAsync("test-key", Arg.Any<CancellationToken>()).Returns(cachedBytes);

        var behavior = new CachingBehavior<CacheableRequest, string>(_cache, _logger);

        var result = await behavior.Handle(new CacheableRequest(), _next, CancellationToken.None);

        result.Should().Be("cached-value");
        await _next.DidNotReceive().Invoke();
    }

    [Fact]
    public async Task Handle_CacheMiss_ExecutesHandlerAndCaches()
    {
        _cache.GetAsync("test-key", Arg.Any<CancellationToken>()).Returns((byte[]?)null);
        _next.Invoke().Returns("fresh-value");

        var behavior = new CachingBehavior<CacheableRequest, string>(_cache, _logger);

        var result = await behavior.Handle(new CacheableRequest(), _next, CancellationToken.None);

        result.Should().Be("fresh-value");
        await _next.Received(1).Invoke();
        await _cache.Received(1).SetAsync(
            "test-key",
            Arg.Any<byte[]>(),
            Arg.Any<DistributedCacheEntryOptions>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CacheSetFails_StillReturnsResponse()
    {
        _cache.GetAsync("test-key", Arg.Any<CancellationToken>()).Returns((byte[]?)null);
        _next.Invoke().Returns("value");
        _cache.SetAsync(Arg.Any<string>(), Arg.Any<byte[]>(),
            Arg.Any<DistributedCacheEntryOptions>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("cache down"));

        var behavior = new CachingBehavior<CacheableRequest, string>(_cache, _logger);

        var result = await behavior.Handle(new CacheableRequest(), _next, CancellationToken.None);

        result.Should().Be("value");
    }

    [Fact]
    public async Task Handle_CacheMiss_SetsCorrectExpiration()
    {
        _cache.GetAsync("test-key", Arg.Any<CancellationToken>()).Returns((byte[]?)null);
        _next.Invoke().Returns("value");

        var behavior = new CachingBehavior<CacheableRequest, string>(_cache, _logger);
        await behavior.Handle(new CacheableRequest(), _next, CancellationToken.None);

        await _cache.Received(1).SetAsync(
            "test-key",
            Arg.Any<byte[]>(),
            Arg.Is<DistributedCacheEntryOptions>(o =>
                o.AbsoluteExpirationRelativeToNow == TimeSpan.FromMinutes(5)),
            Arg.Any<CancellationToken>());
    }
}
