using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using SwiggyClone.Api.Middleware;
using SwiggyClone.Shared.Constants;
using SwiggyClone.UnitTests.Common;

namespace SwiggyClone.UnitTests.Api.Middleware;

public sealed class CorrelationIdMiddlewareTests
{
    private static (DefaultHttpContext Context, TestHttpResponseFeature ResponseFeature) CreateTestContext()
    {
        var responseFeature = new TestHttpResponseFeature();
        var features = new FeatureCollection();
        features.Set<IHttpResponseFeature>(responseFeature);
        features.Set<IHttpRequestFeature>(new HttpRequestFeature());
        return (new DefaultHttpContext(features), responseFeature);
    }

    [Fact]
    public async Task InvokeAsync_HeaderPresent_UsesExistingCorrelationId()
    {
        var (context, responseFeature) = CreateTestContext();
        var expectedId = "existing-correlation-id";
        context.Request.Headers[AppConstants.CorrelationIdHeader] = expectedId;

        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);
        await responseFeature.FireOnStartingAsync();

        context.Response.Headers[AppConstants.CorrelationIdHeader]
            .ToString().Should().Be(expectedId);
    }

    [Fact]
    public async Task InvokeAsync_NoHeader_GeneratesNewCorrelationId()
    {
        var (context, responseFeature) = CreateTestContext();
        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);
        await responseFeature.FireOnStartingAsync();

        var correlationId = context.Response.Headers[AppConstants.CorrelationIdHeader].ToString();
        correlationId.Should().NotBeNullOrWhiteSpace();
        Guid.TryParse(correlationId, out _).Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_ResponseContainsCorrelationIdHeader()
    {
        var (context, responseFeature) = CreateTestContext();
        context.Request.Headers[AppConstants.CorrelationIdHeader] = "test-123";
        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);
        await responseFeature.FireOnStartingAsync();

        context.Response.Headers.Should().ContainKey(AppConstants.CorrelationIdHeader);
    }
}
