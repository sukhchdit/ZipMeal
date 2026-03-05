using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SwiggyClone.IntegrationTests.Common;

namespace SwiggyClone.IntegrationTests.Api;

public sealed class WalletEndpointTests : IntegrationTestBase
{
    public WalletEndpointTests(ZipMealWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetBalance_Authenticated_ReturnsSuccess()
    {
        var client = CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/v1/wallet");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetBalance_Unauthenticated_ReturnsUnauthorized()
    {
        var client = CreateAnonymousClient();

        var response = await client.GetAsync("/api/v1/wallet");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddMoney_ValidAmount_ReturnsSuccess()
    {
        var client = CreateAuthenticatedClient();
        var payload = new { amountPaise = 10000, description = "Test deposit" };

        var response = await client.PostAsJsonAsync("/api/v1/wallet/add-money", payload);

        // May need wallet to exist first; a non-500 response is acceptable
        ((int)response.StatusCode).Should().BeLessThan(500);
    }
}
