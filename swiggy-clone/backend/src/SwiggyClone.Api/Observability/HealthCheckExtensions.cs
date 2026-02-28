namespace SwiggyClone.Api.Observability;

/// <summary>
/// Registers enhanced health checks for all infrastructure dependencies.
/// </summary>
public static class HealthCheckExtensions
{
    public static IServiceCollection AddEnhancedHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddNpgSql(
                configuration.GetConnectionString("DefaultConnection")!,
                name: "postgresql",
                tags: ["db", "ready"])
            .AddRedis(
                configuration.GetConnectionString("Redis")!,
                name: "redis",
                tags: ["cache", "ready"])
            .AddKafka(
                new Confluent.Kafka.ProducerConfig
                {
                    BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092"
                },
                name: "kafka",
                tags: ["messaging", "ready"])
            .AddElasticsearch(
                configuration["Elasticsearch:Uri"] ?? "http://localhost:9200",
                name: "elasticsearch",
                tags: ["search", "ready"]);

        return services;
    }
}
