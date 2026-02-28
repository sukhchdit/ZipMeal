using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Infrastructure.Services;

internal sealed class KafkaTopicInitializer(
    IConfiguration configuration,
    ILogger<KafkaTopicInitializer> logger) : BackgroundService
{
    private static readonly string[] AllTopics =
    [
        KafkaTopics.OrderCreated,
        KafkaTopics.OrderConfirmed,
        KafkaTopics.OrderPreparing,
        KafkaTopics.OrderReady,
        KafkaTopics.OrderDelivered,
        KafkaTopics.OrderCancelled,
        KafkaTopics.PaymentCompleted,
        KafkaTopics.PaymentFailed,
        KafkaTopics.DeliveryAssigned,
        KafkaTopics.DeliveryLocationUpdated,
        KafkaTopics.DineInSessionStarted,
        KafkaTopics.DineInOrderPlaced,
        KafkaTopics.DineInBillRequested,
        KafkaTopics.NotificationSend,
        KafkaTopics.RestaurantMenuUpdated
    ];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

            var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";

            using var adminClient = new AdminClientBuilder(
                new AdminClientConfig { BootstrapServers = bootstrapServers }).Build();

            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
            var existingTopics = metadata.Topics.Select(t => t.Topic).ToHashSet(StringComparer.Ordinal);

            var topicsToCreate = AllTopics
                .Where(t => !existingTopics.Contains(t))
                .Select(t => new TopicSpecification
                {
                    Name = t,
                    NumPartitions = 1,
                    ReplicationFactor = 1
                })
                .ToList();

            if (topicsToCreate.Count == 0)
            {
                logger.LogInformation("All {Count} Kafka topics already exist", AllTopics.Length);
                return;
            }

            await adminClient.CreateTopicsAsync(topicsToCreate);
            logger.LogInformation("Created {Count} Kafka topics: {Topics}",
                topicsToCreate.Count, string.Join(", ", topicsToCreate.Select(t => t.Name)));
        }
        catch (OperationCanceledException)
        {
            // Shutdown requested
        }
        catch (CreateTopicsException ex)
        {
            foreach (var result in ex.Results.Where(r => r.Error.Code != ErrorCode.TopicAlreadyExists))
            {
                logger.LogWarning("Failed to create Kafka topic {Topic}: {Error}",
                    result.Topic, result.Error.Reason);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to initialize Kafka topics — Kafka may be unavailable");
        }
    }
}
