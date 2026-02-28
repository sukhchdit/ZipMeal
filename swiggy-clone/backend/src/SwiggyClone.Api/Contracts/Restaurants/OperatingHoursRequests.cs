namespace SwiggyClone.Api.Contracts.Restaurants;

public sealed record UpsertOperatingHoursRequest(List<OperatingHourEntryRequest> Hours);
public sealed record OperatingHourEntryRequest(short DayOfWeek, TimeOnly? OpenTime, TimeOnly? CloseTime, bool IsClosed);
