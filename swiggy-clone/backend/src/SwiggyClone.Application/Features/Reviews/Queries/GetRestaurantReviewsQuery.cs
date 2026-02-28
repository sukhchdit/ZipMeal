using MediatR;
using SwiggyClone.Application.Features.Reviews.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Queries;

public sealed record GetRestaurantReviewsQuery(
    Guid RestaurantId,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<ReviewDto>>>;
