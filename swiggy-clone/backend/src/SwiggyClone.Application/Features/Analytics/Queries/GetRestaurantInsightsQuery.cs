using MediatR;
using SwiggyClone.Application.Features.Analytics.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Analytics.Queries;

public sealed record GetRestaurantInsightsQuery(
    Guid RestaurantId, Guid OwnerId, string Period, int Days)
    : IRequest<Result<RestaurantInsightsDto>>;
