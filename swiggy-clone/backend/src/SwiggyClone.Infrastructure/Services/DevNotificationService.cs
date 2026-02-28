using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;

namespace SwiggyClone.Infrastructure.Services;

internal sealed class DevNotificationService : INotificationService
{
    private readonly ILogger<DevNotificationService> _logger;

    public DevNotificationService(ILogger<DevNotificationService> logger)
    {
        _logger = logger;
    }

    public async Task<PushResult> SendPushAsync(
        Guid userId,
        string title,
        string body,
        string? data,
        CancellationToken ct = default)
    {
        await Task.Delay(50, ct);

        _logger.LogInformation(
            "[DEV] Push notification sent to UserId={UserId} | Title={Title} | Body={Body} | Data={Data}",
            userId, title, body, data);

        return new PushResult(true, 1, null);
    }
}
