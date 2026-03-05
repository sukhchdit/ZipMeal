using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using SwiggyClone.Api.Middleware;
using SwiggyClone.UnitTests.Common;

namespace SwiggyClone.UnitTests.Api.Middleware;

public sealed class SecurityHeadersMiddlewareTests
{
    private static async Task<HttpContext> ExecuteMiddleware()
    {
        var responseFeature = new TestHttpResponseFeature();
        var features = new FeatureCollection();
        features.Set<IHttpResponseFeature>(responseFeature);
        features.Set<IHttpRequestFeature>(new HttpRequestFeature());
        var context = new DefaultHttpContext(features);

        var middleware = new SecurityHeadersMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);
        await responseFeature.FireOnStartingAsync();

        return context;
    }

    [Fact]
    public async Task InvokeAsync_SetsXFrameOptions()
    {
        var context = await ExecuteMiddleware();
        context.Response.Headers["X-Frame-Options"].ToString().Should().Be("DENY");
    }

    [Fact]
    public async Task InvokeAsync_SetsXContentTypeOptions()
    {
        var context = await ExecuteMiddleware();
        context.Response.Headers["X-Content-Type-Options"].ToString().Should().Be("nosniff");
    }

    [Fact]
    public async Task InvokeAsync_SetsXXSSProtection()
    {
        var context = await ExecuteMiddleware();
        context.Response.Headers["X-XSS-Protection"].ToString().Should().Be("1; mode=block");
    }

    [Fact]
    public async Task InvokeAsync_SetsReferrerPolicy()
    {
        var context = await ExecuteMiddleware();
        context.Response.Headers["Referrer-Policy"].ToString().Should().Be("strict-origin-when-cross-origin");
    }

    [Fact]
    public async Task InvokeAsync_SetsPermissionsPolicy()
    {
        var context = await ExecuteMiddleware();
        context.Response.Headers["Permissions-Policy"].ToString()
            .Should().Be("camera=(), microphone=(), geolocation=(self), payment=()");
    }

    [Fact]
    public async Task InvokeAsync_SetsContentSecurityPolicy()
    {
        var context = await ExecuteMiddleware();
        context.Response.Headers["Content-Security-Policy"].ToString()
            .Should().Be("default-src 'none'; frame-ancestors 'none'");
    }
}
