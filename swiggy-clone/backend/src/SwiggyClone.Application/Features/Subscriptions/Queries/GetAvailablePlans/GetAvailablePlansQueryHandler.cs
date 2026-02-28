using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Subscriptions.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Subscriptions.Queries.GetAvailablePlans;

internal sealed class GetAvailablePlansQueryHandler(IAppDbContext db)
    : IRequestHandler<GetAvailablePlansQuery, Result<List<SubscriptionPlanDto>>>
{
    public async Task<Result<List<SubscriptionPlanDto>>> Handle(GetAvailablePlansQuery request, CancellationToken ct)
    {
        var plans = await db.SubscriptionPlans
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.PricePaise)
            .Select(p => new SubscriptionPlanDto(
                p.Id, p.Name, p.Description, p.PricePaise, p.DurationDays,
                p.FreeDelivery, p.ExtraDiscountPercent, p.NoSurgeFee, p.IsActive))
            .ToListAsync(ct);

        return Result<List<SubscriptionPlanDto>>.Success(plans);
    }
}
