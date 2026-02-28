namespace SwiggyClone.Api.Contracts.Notifications;

public sealed record RegisterDeviceRequest(string DeviceToken, int Platform);

public sealed record UnregisterDeviceRequest(string DeviceToken);
