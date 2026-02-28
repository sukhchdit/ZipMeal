using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using SwiggyClone.Infrastructure.Diagnostics;

namespace SwiggyClone.Infrastructure.Resilience;

/// <summary>
/// Static factory for building Polly v8 resilience pipelines with retry, circuit breaker, and timeout.
/// </summary>
public static class ResiliencePipelineFactory
{
    public static ResiliencePipeline CreateElasticsearchPipeline(
        ElasticsearchResilienceOptions options,
        ILogger logger)
    {
        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = options.RetryCount,
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                Delay = TimeSpan.FromSeconds(options.RetryBaseDelaySeconds),
                OnRetry = args =>
                {
                    InfrastructureDiagnostics.ResilienceRetries.Add(1,
                        new KeyValuePair<string, object?>("service", "elasticsearch"),
                        new KeyValuePair<string, object?>("attempt", args.AttemptNumber));
                    logger.LogWarning(
                        args.Outcome.Exception,
                        "Elasticsearch retry attempt {Attempt} after {Delay}ms",
                        args.AttemptNumber,
                        args.RetryDelay.TotalMilliseconds);
                    return default;
                }
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                FailureRatio = 1.0,
                MinimumThroughput = options.CircuitBreakerFailureThreshold,
                SamplingDuration = TimeSpan.FromSeconds(options.CircuitBreakerDurationSeconds * 2),
                BreakDuration = TimeSpan.FromSeconds(options.CircuitBreakerDurationSeconds),
                OnOpened = args =>
                {
                    InfrastructureDiagnostics.CircuitBreakerStateChanges.Add(1,
                        new KeyValuePair<string, object?>("service", "elasticsearch"),
                        new KeyValuePair<string, object?>("state", "opened"));
                    logger.LogWarning("Elasticsearch circuit breaker OPENED for {Duration}s",
                        options.CircuitBreakerDurationSeconds);
                    return default;
                },
                OnClosed = _ =>
                {
                    InfrastructureDiagnostics.CircuitBreakerStateChanges.Add(1,
                        new KeyValuePair<string, object?>("service", "elasticsearch"),
                        new KeyValuePair<string, object?>("state", "closed"));
                    logger.LogInformation("Elasticsearch circuit breaker CLOSED");
                    return default;
                },
                OnHalfOpened = _ =>
                {
                    InfrastructureDiagnostics.CircuitBreakerStateChanges.Add(1,
                        new KeyValuePair<string, object?>("service", "elasticsearch"),
                        new KeyValuePair<string, object?>("state", "half-opened"));
                    logger.LogInformation("Elasticsearch circuit breaker HALF-OPENED");
                    return default;
                }
            })
            .AddTimeout(new TimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds),
                OnTimeout = args =>
                {
                    InfrastructureDiagnostics.ResilienceTimeouts.Add(1,
                        new KeyValuePair<string, object?>("service", "elasticsearch"));
                    logger.LogWarning("Elasticsearch operation timed out after {Timeout}s",
                        options.TimeoutSeconds);
                    return default;
                }
            })
            .Build();
    }

    public static ResiliencePipeline CreateKafkaPipeline(
        KafkaResilienceOptions options,
        ILogger logger)
    {
        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = options.RetryCount,
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                Delay = TimeSpan.FromSeconds(options.RetryBaseDelaySeconds),
                OnRetry = args =>
                {
                    InfrastructureDiagnostics.ResilienceRetries.Add(1,
                        new KeyValuePair<string, object?>("service", "kafka"),
                        new KeyValuePair<string, object?>("attempt", args.AttemptNumber));
                    logger.LogWarning(
                        args.Outcome.Exception,
                        "Kafka retry attempt {Attempt} after {Delay}ms",
                        args.AttemptNumber,
                        args.RetryDelay.TotalMilliseconds);
                    return default;
                }
            })
            .Build();
    }

    public static ResiliencePipeline CreateRedisCartPipeline(
        RedisCartResilienceOptions options,
        ILogger logger)
    {
        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = options.RetryCount,
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                Delay = TimeSpan.FromSeconds(options.RetryBaseDelaySeconds),
                OnRetry = args =>
                {
                    InfrastructureDiagnostics.ResilienceRetries.Add(1,
                        new KeyValuePair<string, object?>("service", "redis-cart"),
                        new KeyValuePair<string, object?>("attempt", args.AttemptNumber));
                    logger.LogWarning(
                        args.Outcome.Exception,
                        "Redis cart retry attempt {Attempt} after {Delay}ms",
                        args.AttemptNumber,
                        args.RetryDelay.TotalMilliseconds);
                    return default;
                }
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                FailureRatio = 1.0,
                MinimumThroughput = options.CircuitBreakerFailureThreshold,
                SamplingDuration = TimeSpan.FromSeconds(options.CircuitBreakerDurationSeconds * 2),
                BreakDuration = TimeSpan.FromSeconds(options.CircuitBreakerDurationSeconds),
                OnOpened = args =>
                {
                    InfrastructureDiagnostics.CircuitBreakerStateChanges.Add(1,
                        new KeyValuePair<string, object?>("service", "redis-cart"),
                        new KeyValuePair<string, object?>("state", "opened"));
                    logger.LogWarning("Redis cart circuit breaker OPENED for {Duration}s",
                        options.CircuitBreakerDurationSeconds);
                    return default;
                },
                OnClosed = _ =>
                {
                    InfrastructureDiagnostics.CircuitBreakerStateChanges.Add(1,
                        new KeyValuePair<string, object?>("service", "redis-cart"),
                        new KeyValuePair<string, object?>("state", "closed"));
                    logger.LogInformation("Redis cart circuit breaker CLOSED");
                    return default;
                },
                OnHalfOpened = _ =>
                {
                    InfrastructureDiagnostics.CircuitBreakerStateChanges.Add(1,
                        new KeyValuePair<string, object?>("service", "redis-cart"),
                        new KeyValuePair<string, object?>("state", "half-opened"));
                    logger.LogInformation("Redis cart circuit breaker HALF-OPENED");
                    return default;
                }
            })
            .AddTimeout(new TimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds),
                OnTimeout = args =>
                {
                    InfrastructureDiagnostics.ResilienceTimeouts.Add(1,
                        new KeyValuePair<string, object?>("service", "redis-cart"));
                    logger.LogWarning("Redis cart operation timed out after {Timeout}s",
                        options.TimeoutSeconds);
                    return default;
                }
            })
            .Build();
    }
}
