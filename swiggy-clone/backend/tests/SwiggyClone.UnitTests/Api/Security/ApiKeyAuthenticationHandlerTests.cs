using System.Text.Encodings.Web;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using SwiggyClone.Api.Security;

namespace SwiggyClone.UnitTests.Api.Security;

public sealed class ApiKeyAuthenticationHandlerTests
{
    private static readonly string[] ValidKeyArray = ["valid-key"];

    private static async Task<AuthenticateResult> Authenticate(
        string? apiKeyHeader, string[]? configuredKeys)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(BuildConfigEntries(configuredKeys))
            .Build();

        var options = new ApiKeyAuthenticationOptions();
        var optionsMonitor = Substitute.For<IOptionsMonitor<ApiKeyAuthenticationOptions>>();
        optionsMonitor.Get(Arg.Any<string>()).Returns(options);
        optionsMonitor.CurrentValue.Returns(options);

        var loggerFactory = Substitute.For<ILoggerFactory>();
        loggerFactory.CreateLogger(Arg.Any<string>()).Returns(Substitute.For<ILogger>());

        var handler = new ApiKeyAuthenticationHandler(
            optionsMonitor, loggerFactory, UrlEncoder.Default, config);

        var scheme = new AuthenticationScheme("ApiKey", null, typeof(ApiKeyAuthenticationHandler));
        var context = new DefaultHttpContext();
        if (apiKeyHeader is not null)
        {
            context.Request.Headers["X-Api-Key"] = apiKeyHeader;
        }

        await handler.InitializeAsync(scheme, context);
        return await handler.AuthenticateAsync();
    }

    private static IEnumerable<KeyValuePair<string, string?>> BuildConfigEntries(string[]? keys)
    {
        if (keys is null) yield break;
        for (int i = 0; i < keys.Length; i++)
        {
            yield return new KeyValuePair<string, string?>($"Security:ApiKeys:{i}", keys[i]);
        }
    }

    [Fact]
    public async Task HandleAuthenticate_NoHeader_ReturnsNoResult()
    {
        var result = await Authenticate(null, ValidKeyArray);

        result.None.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAuthenticate_InvalidKey_ReturnsFail()
    {
        var result = await Authenticate("wrong-key", ValidKeyArray);

        result.Succeeded.Should().BeFalse();
        result.None.Should().BeFalse();
    }

    [Fact]
    public async Task HandleAuthenticate_ValidKey_ReturnsSuccess()
    {
        var result = await Authenticate("valid-key", ValidKeyArray);

        result.Succeeded.Should().BeTrue();
        result.Principal!.Identity!.IsAuthenticated.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAuthenticate_EmptyConfiguredKeys_ReturnsFail()
    {
        var result = await Authenticate("some-key", Array.Empty<string>());

        result.Succeeded.Should().BeFalse();
    }
}
