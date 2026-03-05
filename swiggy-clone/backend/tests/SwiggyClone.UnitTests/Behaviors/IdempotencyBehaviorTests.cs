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

public sealed class IdempotencyBehaviorTests
{
    public sealed record NonIdempotentRequest : IRequest<string>;

    public sealed record IdempotentRequest(string? IdempotencyKey) : IRequest<string>, IIdempotent;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    private readonly IDistributedCache _cache = Substitute.For<IDistributedCache>();

    private readonly ILogger<IdempotencyBehavior<IdempotentRequest, string>> _logger =
        Substitute.For<ILogger<IdempotencyBehavior<IdempotentRequest, string>>>();

    private readonly RequestHandlerDelegate<string> _next = Substitute.For<RequestHandlerDelegate<string>>();

    [Fact]
    public async Task Handle_NonIdempotentRequest_BypassesCheck()
    {
        var nonIdempotentNext = Substitute.For<RequestHandlerDelegate<string>>();
        nonIdempotentNext.Invoke().Returns("direct");
        var nonIdempotentCache = Substitute.For<IDistributedCache>();
        var nonIdempotentLogger =
            Substitute.For<ILogger<IdempotencyBehavior<NonIdempotentRequest, string>>>();

        var behavior = new IdempotencyBehavior<NonIdempotentRequest, string>(nonIdempotentCache, nonIdempotentLogger);

        var result = await behavior.Handle(new NonIdempotentRequest(), nonIdempotentNext, CancellationToken.None);

        result.Should().Be("direct");
        await nonIdempotentCache.DidNotReceive().GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NullIdempotencyKey_BypassesCheck()
    {
        _next.Invoke().Returns("direct");

        var behavior = new IdempotencyBehavior<IdempotentRequest, string>(_cache, _logger);

        var result = await behavior.Handle(new IdempotentRequest(null), _next, CancellationToken.None);

        result.Should().Be("direct");
        await _cache.DidNotReceive().GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DuplicateKey_ReturnsCachedResult()
    {
        var cachedBytes = JsonSerializer.SerializeToUtf8Bytes("cached-value", JsonOptions);
        _cache.GetAsync("idempotency:IdempotentRequest:order-123", Arg.Any<CancellationToken>())
            .Returns(cachedBytes);

        var behavior = new IdempotencyBehavior<IdempotentRequest, string>(_cache, _logger);

        var result = await behavior.Handle(
            new IdempotentRequest("order-123"), _next, CancellationToken.None);

        result.Should().Be("cached-value");
        await _next.DidNotReceive().Invoke();
    }

    [Fact]
    public async Task Handle_NewKey_ExecutesHandlerAndCaches()
    {
        _cache.GetAsync("idempotency:IdempotentRequest:order-456", Arg.Any<CancellationToken>())
            .Returns((byte[]?)null);
        _next.Invoke().Returns("fresh-value");

        var behavior = new IdempotencyBehavior<IdempotentRequest, string>(_cache, _logger);

        var result = await behavior.Handle(
            new IdempotentRequest("order-456"), _next, CancellationToken.None);

        result.Should().Be("fresh-value");
        await _next.Received(1).Invoke();
        await _cache.Received(1).SetAsync(
            "idempotency:IdempotentRequest:order-456",
            Arg.Any<byte[]>(),
            Arg.Any<DistributedCacheEntryOptions>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CacheSetFails_StillReturnsResponse()
    {
        _cache.GetAsync("idempotency:IdempotentRequest:order-789", Arg.Any<CancellationToken>())
            .Returns((byte[]?)null);
        _next.Invoke().Returns("value");
        _cache.SetAsync(Arg.Any<string>(), Arg.Any<byte[]>(),
            Arg.Any<DistributedCacheEntryOptions>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("cache down"));

        var behavior = new IdempotencyBehavior<IdempotentRequest, string>(_cache, _logger);

        var result = await behavior.Handle(
            new IdempotentRequest("order-789"), _next, CancellationToken.None);

        result.Should().Be("value");
    }
}
