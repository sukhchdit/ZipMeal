namespace SwiggyClone.IntegrationTests.Common;

public abstract class IntegrationTestBase : IClassFixture<ZipMealWebApplicationFactory>
{
    protected HttpClient Client { get; }
    protected ZipMealWebApplicationFactory Factory { get; }

    protected IntegrationTestBase(ZipMealWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    protected HttpClient CreateAuthenticatedClient()
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(TestAuthHandler.SchemeName, "test-token");
        return client;
    }

    protected HttpClient CreateAnonymousClient()
    {
        return Factory.CreateClient();
    }
}
