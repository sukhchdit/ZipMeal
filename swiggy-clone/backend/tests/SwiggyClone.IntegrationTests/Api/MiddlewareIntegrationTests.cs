using System.Net;
using FluentAssertions;
using SwiggyClone.IntegrationTests.Common;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.IntegrationTests.Api;

public sealed class MiddlewareIntegrationTests : IntegrationTestBase
{
    public MiddlewareIntegrationTests(ZipMealWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Response_ContainsSecurityHeaders()
    {
        var response = await Client.GetAsync("/health");

        response.Headers.Should().Contain(h => h.Key == "X-Content-Type-Options");
    }

    [Fact]
    public async Task Response_ContainsCorrelationIdHeader()
    {
        var response = await Client.GetAsync("/health");

        response.Headers.Should().Contain(h => h.Key == AppConstants.CorrelationIdHeader);
    }

    [Fact]
    public async Task NonExistentRoute_ReturnsNotFoundWithJson()
    {
        var response = await Client.GetAsync("/api/v1/nonexistent-endpoint-xyz");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
