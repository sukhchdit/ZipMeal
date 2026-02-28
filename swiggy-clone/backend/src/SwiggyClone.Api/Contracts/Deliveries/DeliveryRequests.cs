namespace SwiggyClone.Api.Contracts.Deliveries;

public sealed record ToggleOnlineRequest(bool IsOnline, double? Latitude, double? Longitude);

public sealed record UpdateDeliveryStatusRequest(int NewStatus);

public sealed record UpdateLocationRequest(double Latitude, double Longitude, double? Heading, double? Speed);
