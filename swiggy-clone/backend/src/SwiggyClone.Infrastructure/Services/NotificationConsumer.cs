using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Infrastructure.Services;

internal sealed record SendNotificationMessage(Guid UserId, string Title, string Body, string? Data);

internal sealed class NotificationConsumer(
    IConfiguration configuration,
    IServiceScopeFactory scopeFactory,
    ILogger<NotificationConsumer> logger)
    : KafkaConsumerBase<SendNotificationMessage>(
        configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
        configuration["Kafka:NotificationConsumerGroupId"] ?? "swiggyclone-notifications",
        KafkaTopics.NotificationSend,
        logger)
{
    protected override async Task ProcessAsync(string key, SendNotificationMessage message, CancellationToken ct)
    {
        logger.LogDebug("Processing push notification for user {UserId}: {Title}", message.UserId, message.Title);

        using var scope = scopeFactory.CreateScope();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        var result = await notificationService.SendPushAsync(
            message.UserId, message.Title, message.Body, message.Data, ct);

        if (!result.Success)
        {
            logger.LogWarning("Push notification failed for user {UserId}: {Error}", message.UserId, result.Error);
        }
    }
}
