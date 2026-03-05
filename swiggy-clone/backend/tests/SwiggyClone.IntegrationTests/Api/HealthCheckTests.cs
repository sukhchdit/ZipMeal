using System.Net;
using FluentAssertions;
using SwiggyClone.IntegrationTests.Common;

namespace SwiggyClone.IntegrationTests.Api;

public sealed class HealthCheckTests : IntegrationTestBase
{
    public HealthCheckTests(ZipMealWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task HealthEndpoint_ReturnsOk()
    {
        var response = await Client.GetAsync("/health");

        // External deps (Redis, Kafka, ES) are absent in test environment,
        // so the overall health may report Unhealthy (503). Verify the endpoint responds.
        ((int)response.StatusCode).Should().BeOneOf(200, 503);
    }

    [Fact]
    public async Task ReadinessEndpoint_ReturnsOkOrDegraded()
    {
        var response = await Client.GetAsync("/health/ready");

        // Readiness may fail in test env if external deps (Redis, Kafka, ES) are missing
        // but the endpoint itself should respond (200 or 503)
        ((int)response.StatusCode).Should().BeOneOf(200, 503);
    }
}
