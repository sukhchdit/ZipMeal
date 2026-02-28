using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SwiggyClone.Infrastructure.Services;

internal abstract class KafkaConsumerBase<TMessage>(
    string bootstrapServers,
    string groupId,
    string topic,
    ILogger logger) : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Latest,
            EnableAutoCommit = true,
            AutoCommitIntervalMs = 5000
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(topic);

        logger.LogInformation("Kafka consumer started for topic {Topic} (group: {GroupId})", topic, groupId);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(TimeSpan.FromSeconds(1));
                    if (result is null) continue;

                    var message = JsonSerializer.Deserialize<TMessage>(result.Message.Value, JsonOptions);
                    if (message is not null)
                    {
                        await ProcessAsync(result.Message.Key, message, stoppingToken);
                    }
                }
                catch (ConsumeException ex)
                {
                    logger.LogWarning(ex, "Kafka consume error on topic {Topic}", topic);
                }
                catch (JsonException ex)
                {
                    logger.LogWarning(ex, "Failed to deserialize Kafka message from topic {Topic}", topic);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    logger.LogWarning(ex, "Error processing Kafka message from topic {Topic}", topic);
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Shutdown
        }
        finally
        {
            consumer.Close();
        }
    }

    protected abstract Task ProcessAsync(string key, TMessage message, CancellationToken ct);
}
