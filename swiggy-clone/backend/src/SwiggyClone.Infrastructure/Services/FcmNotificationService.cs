using System.Text.Json;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Infrastructure.Persistence;

namespace SwiggyClone.Infrastructure.Services;

internal sealed class FcmNotificationService(
    AppDbContext db,
    ILogger<FcmNotificationService> logger) : INotificationService
{
    public async Task<PushResult> SendPushAsync(
        Guid userId,
        string title,
        string body,
        string? data,
        CancellationToken ct = default)
    {
        var tokens = await db.UserDevices
            .Where(d => d.UserId == userId && d.IsActive)
            .Select(d => d.DeviceToken)
            .ToListAsync(ct);

        if (tokens.Count == 0)
        {
            logger.LogDebug("No active devices for UserId={UserId}, skipping push", userId);
            return new PushResult(true, 0, null);
        }

        var messageData = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(data))
        {
            try
            {
                messageData = JsonSerializer.Deserialize<Dictionary<string, string>>(data)
                    ?? new Dictionary<string, string>();
            }
            catch (JsonException ex)
            {
                logger.LogWarning(ex, "Failed to deserialize push data for UserId={UserId}", userId);
            }
        }

        var message = new MulticastMessage
        {
            Tokens = tokens,
            Notification = new Notification
            {
                Title = title,
                Body = body,
            },
            Data = messageData,
        };

        var response = await FirebaseMessaging.DefaultInstance
            .SendEachForMulticastAsync(message, ct);

        // Deactivate stale tokens
        var staleTokens = new List<string>();
        for (var i = 0; i < response.Responses.Count; i++)
        {
            if (!response.Responses[i].IsSuccess &&
                response.Responses[i].Exception?.MessagingErrorCode == MessagingErrorCode.Unregistered)
            {
                staleTokens.Add(tokens[i]);
            }
        }

        if (staleTokens.Count > 0)
        {
            await db.UserDevices
                .Where(d => staleTokens.Contains(d.DeviceToken) && d.UserId == userId)
                .ExecuteUpdateAsync(s => s.SetProperty(d => d.IsActive, false), ct);

            logger.LogInformation(
                "Deactivated {Count} stale FCM tokens for UserId={UserId}",
                staleTokens.Count, userId);
        }

        var successCount = response.SuccessCount;
        string? failureMessage = response.FailureCount > 0
            ? $"{response.FailureCount} of {tokens.Count} devices failed"
            : null;

        logger.LogInformation(
            "FCM push sent to UserId={UserId}: {SuccessCount}/{Total} succeeded",
            userId, successCount, tokens.Count);

        return new PushResult(successCount > 0, successCount, failureMessage);
    }
}
