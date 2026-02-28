using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Discovery.Documents;
using SwiggyClone.Domain.Common;
using SwiggyClone.Infrastructure.Persistence;
using SwiggyClone.Infrastructure.Persistence.Interceptors;
using SwiggyClone.Infrastructure.Persistence.Repositories;
using SwiggyClone.Infrastructure.Services;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Infrastructure;

/// <summary>
/// Registers all Infrastructure-layer services into the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddInterceptors();
        services.AddDatabase(configuration);
        services.AddRedis(configuration);
        services.AddElasticsearch(configuration);
        services.AddKafka(configuration);
        services.AddRepositories();
        services.AddAuthServices();

        return services;
    }

    private static void AddInterceptors(this IServiceCollection services)
    {
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<SoftDeleteInterceptor>();
    }

    private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var auditInterceptor = serviceProvider.GetRequiredService<AuditableEntityInterceptor>();
            var softDeleteInterceptor = serviceProvider.GetRequiredService<SoftDeleteInterceptor>();

            options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorCodesToAdd: null);
                    })
                .AddInterceptors(auditInterceptor, softDeleteInterceptor);
        });

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<AppDbContext>());
    }

    private static void AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis");

        if (string.IsNullOrWhiteSpace(redisConnectionString))
        {
            return;
        }

        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(redisConnectionString));

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "SwiggyClone:";
        });
    }

    private static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
    {
        var uri = configuration["Elasticsearch:Uri"] ?? "http://localhost:9200";

        var settings = new ElasticsearchClientSettings(new Uri(uri))
            .DefaultMappingFor<RestaurantSearchDocument>(m =>
                m.IndexName(ElasticsearchConstants.RestaurantIndex))
            .DefaultMappingFor<MenuItemSearchDocument>(m =>
                m.IndexName(ElasticsearchConstants.MenuItemIndex))
            .RequestTimeout(TimeSpan.FromSeconds(10));

        var client = new ElasticsearchClient(settings);

        services.AddSingleton(client);
        services.AddSingleton<ISearchService, ElasticsearchService>();
        services.AddHostedService<ElasticsearchIndexInitializer>();
    }

    private static void AddKafka(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IEventBus, KafkaEventBus>();
        services.AddHostedService<KafkaTopicInitializer>();
        services.AddHostedService<NotificationConsumer>();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
    }

    private static void AddAuthServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IOtpService, DevOtpService>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<ICartService, RedisCartService>();
        services.AddScoped<IPaymentGatewayService, DevPaymentGatewayService>();
        services.AddScoped<INotificationService, DevNotificationService>();
    }
}
