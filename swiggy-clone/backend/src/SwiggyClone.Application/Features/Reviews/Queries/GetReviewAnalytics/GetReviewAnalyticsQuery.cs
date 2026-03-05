using MediatR;
using SwiggyClone.Application.Features.Reviews.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Queries.GetReviewAnalytics;

public sealed record GetReviewAnalyticsQuery(
    Guid RestaurantId,
    Guid OwnerId) : IRequest<Result<ReviewAnalyticsDto>>;
