using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Infrastructure.Diagnostics;

namespace SwiggyClone.Infrastructure.Services;

internal sealed class KafkaEventBus : IEventBus, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaEventBus> _logger;
    private readonly ResiliencePipeline _pipeline;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public KafkaEventBus(
        IConfiguration configuration,
        ILogger<KafkaEventBus> logger,
        [FromKeyedServices("kafka")] ResiliencePipeline pipeline)
    {
        _logger = logger;
        _pipeline = pipeline;

        var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";

        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            Acks = Acks.Leader,
            EnableIdempotence = false,
            MessageTimeoutMs = 5000
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync<T>(string topic, string key, T message, CancellationToken ct = default)
    {
        using var activity = InfrastructureDiagnostics.ActivitySource.StartActivity(
            $"Kafka Publish {topic}", ActivityKind.Producer);

        activity?.SetTag("messaging.system", "kafka");
        activity?.SetTag("messaging.destination", topic);
        activity?.SetTag("messaging.destination_kind", "topic");

        try
        {
            var json = JsonSerializer.Serialize(message, JsonOptions);

            var kafkaMessage = new Message<string, string>
            {
                Key = key,
                Value = json,
                Headers = []
            };

            // Propagate W3C trace context via Kafka message headers
            if (activity?.Context is { } ctx)
            {
                var traceparent = $"00-{ctx.TraceId}-{ctx.SpanId}-{(ctx.TraceFlags.HasFlag(ActivityTraceFlags.Recorded) ? "01" : "00")}";
                kafkaMessage.Headers.Add("traceparent", Encoding.UTF8.GetBytes(traceparent));

                if (!string.IsNullOrEmpty(activity.TraceStateString))
                {
                    kafkaMessage.Headers.Add("tracestate", Encoding.UTF8.GetBytes(activity.TraceStateString));
                }
            }

            await _pipeline.ExecuteAsync(async token =>
            {
                var result = await _producer.ProduceAsync(topic, kafkaMessage, token);

                activity?.SetTag("messaging.kafka.partition", result.Partition.Value);
                activity?.SetTag("messaging.kafka.offset", result.Offset.Value);

                _logger.LogDebug("Published to {Topic} partition {Partition} offset {Offset}",
                    topic, result.Partition.Value, result.Offset.Value);
            }, ct);
        }
        catch (ProduceException<string, string> ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            _logger.LogWarning(ex, "Failed to publish event to Kafka topic {Topic}", topic);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            _logger.LogWarning(ex, "Unexpected error publishing to Kafka topic {Topic}", topic);
        }
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
    }
}
