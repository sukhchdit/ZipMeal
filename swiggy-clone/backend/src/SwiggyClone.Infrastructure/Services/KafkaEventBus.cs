using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;

namespace SwiggyClone.Infrastructure.Services;

internal sealed class KafkaEventBus : IEventBus, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaEventBus> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public KafkaEventBus(IConfiguration configuration, ILogger<KafkaEventBus> logger)
    {
        _logger = logger;

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
        try
        {
            var json = JsonSerializer.Serialize(message, JsonOptions);

            var kafkaMessage = new Message<string, string>
            {
                Key = key,
                Value = json
            };

            var result = await _producer.ProduceAsync(topic, kafkaMessage, ct);
            _logger.LogDebug("Published to {Topic} partition {Partition} offset {Offset}",
                topic, result.Partition.Value, result.Offset.Value);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogWarning(ex, "Failed to publish event to Kafka topic {Topic}", topic);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unexpected error publishing to Kafka topic {Topic}", topic);
        }
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
    }
}
