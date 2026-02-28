using MediatR;
using SwiggyClone.Application.Features.Subscriptions.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Subscriptions.Queries.GetPlans;

public sealed record GetPlansQuery(
    string? Search,
    bool? IsActive,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<AdminSubscriptionPlanDto>>>;
