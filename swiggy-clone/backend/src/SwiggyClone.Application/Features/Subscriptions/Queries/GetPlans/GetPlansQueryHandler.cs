using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Subscriptions.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Subscriptions.Queries.GetPlans;

internal sealed class GetPlansQueryHandler(IAppDbContext db)
    : IRequestHandler<GetPlansQuery, Result<PagedResult<AdminSubscriptionPlanDto>>>
{
    public async Task<Result<PagedResult<AdminSubscriptionPlanDto>>> Handle(GetPlansQuery request, CancellationToken ct)
    {
        var query = db.SubscriptionPlans.AsNoTracking().AsQueryable();

        if (request.IsActive.HasValue)
            query = query.Where(p => p.IsActive == request.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = $"%{request.Search}%";
            query = query.Where(p => EF.Functions.Like(p.Name, search));
        }

        query = query.OrderByDescending(p => p.CreatedAt);

        var totalCount = await query.CountAsync(ct);

        var plans = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new AdminSubscriptionPlanDto(
                p.Id, p.Name, p.Description, p.PricePaise, p.DurationDays,
                p.FreeDelivery, p.ExtraDiscountPercent, p.NoSurgeFee, p.IsActive,
                p.CreatedAt, p.UpdatedAt))
            .ToListAsync(ct);

        var result = new PagedResult<AdminSubscriptionPlanDto>(plans, totalCount, request.Page, request.PageSize);
        return Result<PagedResult<AdminSubscriptionPlanDto>>.Success(result);
    }
}
