using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Subscriptions.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Subscriptions.Queries.CheckActiveSubscription;

internal sealed class CheckActiveSubscriptionQueryHandler(IAppDbContext db)
    : IRequestHandler<CheckActiveSubscriptionQuery, Result<ActiveSubscriptionBenefitsDto>>
{
    public async Task<Result<ActiveSubscriptionBenefitsDto>> Handle(CheckActiveSubscriptionQuery request, CancellationToken ct)
    {
        var sub = await db.UserSubscriptions
            .AsNoTracking()
            .Include(s => s.Plan)
            .Where(s => s.UserId == request.UserId
                && s.Status == SubscriptionStatus.Active
                && s.EndDate > DateTimeOffset.UtcNow)
            .FirstOrDefaultAsync(ct);

        if (sub is null)
            return Result<ActiveSubscriptionBenefitsDto>.Success(
                new ActiveSubscriptionBenefitsDto(false, false, 0, false));

        return Result<ActiveSubscriptionBenefitsDto>.Success(
            new ActiveSubscriptionBenefitsDto(true, sub.Plan.FreeDelivery, sub.Plan.ExtraDiscountPercent, sub.Plan.NoSurgeFee));
    }
}
