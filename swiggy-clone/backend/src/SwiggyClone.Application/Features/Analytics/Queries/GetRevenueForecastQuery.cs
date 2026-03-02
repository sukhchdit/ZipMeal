using MediatR;
using SwiggyClone.Application.Features.Analytics.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Analytics.Queries;

public sealed record GetRevenueForecastQuery(
    Guid? RestaurantId, Guid? OwnerId, int Days, int ForecastDays)
    : IRequest<Result<RevenueForecastDto>>;
