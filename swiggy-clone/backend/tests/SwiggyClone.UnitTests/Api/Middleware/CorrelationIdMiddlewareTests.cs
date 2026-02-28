using FluentAssertions;
using Microsoft.AspNetCore.Http;
using SwiggyClone.Api.Middleware;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.UnitTests.Api.Middleware;

public sealed class CorrelationIdMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_HeaderPresent_UsesExistingCorrelationId()
    {
        var context = new DefaultHttpContext();
        var expectedId = "existing-correlation-id";
        context.Request.Headers[AppConstants.CorrelationIdHeader] = expectedId;

        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);

        // Trigger OnStarting callbacks
        await context.Response.StartAsync();

        context.Response.Headers[AppConstants.CorrelationIdHeader]
            .ToString().Should().Be(expectedId);
    }

    [Fact]
    public async Task InvokeAsync_NoHeader_GeneratesNewCorrelationId()
    {
        var context = new DefaultHttpContext();
        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);
        await context.Response.StartAsync();

        var correlationId = context.Response.Headers[AppConstants.CorrelationIdHeader].ToString();
        correlationId.Should().NotBeNullOrWhiteSpace();
        Guid.TryParse(correlationId, out _).Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_ResponseContainsCorrelationIdHeader()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[AppConstants.CorrelationIdHeader] = "test-123";
        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);
        await context.Response.StartAsync();

        context.Response.Headers.Should().ContainKey(AppConstants.CorrelationIdHeader);
    }
}
