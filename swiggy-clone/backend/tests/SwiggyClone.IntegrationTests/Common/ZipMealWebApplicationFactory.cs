using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace SwiggyClone.IntegrationTests.Common;

public sealed class ZipMealWebApplicationFactory : WebApplicationFactory<SwiggyClone.Api.Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("zipmeal_test")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    public string ConnectionString => _postgres.GetConnectionString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            // Override authentication with test handler
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                TestAuthHandler.SchemeName, _ => { });
        });

        builder.UseSetting("ConnectionStrings:DefaultConnection", ConnectionString);
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        // Trigger host creation and ensure the database schema exists.
        // The "Testing" environment skips DataSeeder, so we must create tables explicitly.
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();

        // Seed the test user so authenticated endpoints that touch the DB can resolve FK constraints.
        var testUserId = Guid.Parse(TestAuthHandler.DefaultUserId);
        if (!db.Users.Any(u => u.Id == testUserId))
        {
            db.Users.Add(new User
            {
                Id = testUserId,
                PhoneNumber = "+919000000000",
                Email = "test@example.com",
                FullName = "Test User",
                Role = UserRole.Customer,
                IsVerified = true,
                IsActive = true,
                ReferralCode = "TEST0001",
            });
            await db.SaveChangesAsync();
        }
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }
}
