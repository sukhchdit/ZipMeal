namespace SwiggyClone.Application.Features.Restaurants.DTOs;

public sealed record OperatingHoursDto(
    Guid Id,
    short DayOfWeek,
    TimeOnly? OpenTime,
    TimeOnly? CloseTime,
    bool IsClosed);
