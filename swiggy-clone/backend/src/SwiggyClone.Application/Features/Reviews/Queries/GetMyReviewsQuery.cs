using MediatR;
using SwiggyClone.Application.Features.Reviews.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Queries;

public sealed record GetMyReviewsQuery(
    Guid UserId,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<ReviewDto>>>;
