using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiggyClone.Api.Security;

namespace SwiggyClone.UnitTests.Api.Security;

public sealed class AdminIpWhitelistAttributeTests
{
    private static readonly string[] LocalhostWhitelist = ["127.0.0.1"];
    private static readonly string[] InternalWhitelist = ["10.0.0.1"];
    private static readonly string[] InternalWhitelistAlt = ["10.0.0.5"];

    private static AuthorizationFilterContext CreateContext(
        string[]? whitelist, string? remoteIp, string? xRealIp = null)
    {
        var configEntries = new List<KeyValuePair<string, string?>>();
        if (whitelist is not null)
        {
            for (int i = 0; i < whitelist.Length; i++)
            {
                configEntries.Add(new($"Security:AdminIpWhitelist:{i}", whitelist[i]));
            }
        }

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configEntries)
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(config);
        var serviceProvider = services.BuildServiceProvider();

        var httpContext = new DefaultHttpContext { RequestServices = serviceProvider };

        if (remoteIp is not null)
        {
            httpContext.Connection.RemoteIpAddress = IPAddress.Parse(remoteIp);
        }

        if (xRealIp is not null)
        {
            httpContext.Request.Headers["X-Real-IP"] = xRealIp;
        }

        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        return new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
    }

    [Fact]
    public void OnAuthorization_NoWhitelist_Passes()
    {
        var context = CreateContext(Array.Empty<string>(), "192.168.1.1");
        var attribute = new AdminIpWhitelistAttribute();

        attribute.OnAuthorization(context);

        context.Result.Should().BeNull();
    }

    [Fact]
    public void OnAuthorization_IpInWhitelist_Passes()
    {
        var context = CreateContext(LocalhostWhitelist, "127.0.0.1");
        var attribute = new AdminIpWhitelistAttribute();

        attribute.OnAuthorization(context);

        context.Result.Should().BeNull();
    }

    [Fact]
    public void OnAuthorization_IpNotInWhitelist_Returns403()
    {
        var context = CreateContext(InternalWhitelist, "192.168.1.1");
        var attribute = new AdminIpWhitelistAttribute();

        attribute.OnAuthorization(context);

        context.Result.Should().NotBeNull();
        var objectResult = context.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    [Fact]
    public void OnAuthorization_XRealIpHeader_UsesHeaderValue()
    {
        var context = CreateContext(InternalWhitelistAlt, "192.168.1.1", xRealIp: "10.0.0.5");
        var attribute = new AdminIpWhitelistAttribute();

        attribute.OnAuthorization(context);

        context.Result.Should().BeNull();
    }

    [Fact]
    public void OnAuthorization_NullRemoteIp_Returns403()
    {
        var context = CreateContext(InternalWhitelist, null);
        var attribute = new AdminIpWhitelistAttribute();

        attribute.OnAuthorization(context);

        context.Result.Should().NotBeNull();
        var objectResult = context.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }
}
