namespace SwiggyClone.Infrastructure.Resilience;

public sealed class ElasticsearchResilienceOptions
{
    public const string SectionName = "Resilience:Elasticsearch";
    public int RetryCount { get; set; } = 3;
    public double RetryBaseDelaySeconds { get; set; } = 1;
    public int CircuitBreakerFailureThreshold { get; set; } = 5;
    public int CircuitBreakerDurationSeconds { get; set; } = 30;
    public int TimeoutSeconds { get; set; } = 10;
}

public sealed class KafkaResilienceOptions
{
    public const string SectionName = "Resilience:Kafka";
    public int RetryCount { get; set; } = 2;
    public double RetryBaseDelaySeconds { get; set; } = 0.5;
}

public sealed class RedisCartResilienceOptions
{
    public const string SectionName = "Resilience:RedisCart";
    public int RetryCount { get; set; } = 2;
    public double RetryBaseDelaySeconds { get; set; } = 0.25;
    public int CircuitBreakerFailureThreshold { get; set; } = 5;
    public int CircuitBreakerDurationSeconds { get; set; } = 15;
    public int TimeoutSeconds { get; set; } = 5;
}
