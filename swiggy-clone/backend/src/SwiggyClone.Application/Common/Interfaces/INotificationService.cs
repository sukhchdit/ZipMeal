namespace SwiggyClone.Application.Common.Interfaces;

public sealed record PushResult(bool Success, int DevicesSent, string? Error);

public interface INotificationService
{
    Task<PushResult> SendPushAsync(
        Guid userId,
        string title,
        string body,
        string? data,
        CancellationToken ct = default);
}
