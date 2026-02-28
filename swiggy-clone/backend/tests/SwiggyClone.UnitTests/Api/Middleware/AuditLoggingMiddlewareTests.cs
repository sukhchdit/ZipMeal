using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SwiggyClone.Api.Middleware;

namespace SwiggyClone.UnitTests.Api.Middleware;

public sealed class AuditLoggingMiddlewareTests
{
    private readonly ILogger<AuditLoggingMiddleware> _logger =
        Substitute.For<ILogger<AuditLoggingMiddleware>>();

    [Fact]
    public async Task InvokeAsync_GetRequest_DoesNotLog()
    {
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/api/v1/admin/users";
        var middleware = new AuditLoggingMiddleware(_ => Task.CompletedTask, _logger);

        await middleware.InvokeAsync(context);

        _logger.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public async Task InvokeAsync_PostAuditedPath_Logs()
    {
        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.Path = "/api/v1/auth/login";
        context.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");
        var middleware = new AuditLoggingMiddleware(_ => Task.CompletedTask, _logger);

        await middleware.InvokeAsync(context);

        _logger.ReceivedCalls().Should().NotBeEmpty();
    }

    [Fact]
    public async Task InvokeAsync_PostNonAuditedPath_DoesNotLog()
    {
        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.Path = "/api/v1/restaurants";
        var middleware = new AuditLoggingMiddleware(_ => Task.CompletedTask, _logger);

        await middleware.InvokeAsync(context);

        _logger.ReceivedCalls().Should().BeEmpty();
    }
}
