using MediatR;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

public sealed record UpsertOperatingHoursCommand(
    Guid RestaurantId,
    Guid OwnerId,
    List<OperatingHourEntry> Hours) : IRequest<Result<List<OperatingHoursDto>>>;

public sealed record OperatingHourEntry(
    short DayOfWeek,
    TimeOnly? OpenTime,
    TimeOnly? CloseTime,
    bool IsClosed);
