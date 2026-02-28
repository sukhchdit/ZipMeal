using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Subscriptions.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Subscriptions.Queries.GetMySubscription;

internal sealed class GetMySubscriptionQueryHandler(IAppDbContext db)
    : IRequestHandler<GetMySubscriptionQuery, Result<UserSubscriptionDto?>>
{
    public async Task<Result<UserSubscriptionDto?>> Handle(GetMySubscriptionQuery request, CancellationToken ct)
    {
        var sub = await db.UserSubscriptions
            .AsNoTracking()
            .Include(s => s.Plan)
            .Where(s => s.UserId == request.UserId
                && s.Status == SubscriptionStatus.Active
                && s.EndDate > DateTimeOffset.UtcNow)
            .FirstOrDefaultAsync(ct);

        if (sub is null)
            return Result<UserSubscriptionDto?>.Success(null);

        var dto = new UserSubscriptionDto(
            sub.Id, sub.PlanId, sub.Plan.Name, sub.PaidAmountPaise,
            sub.StartDate, sub.EndDate, (int)sub.Status,
            sub.Plan.FreeDelivery, sub.Plan.ExtraDiscountPercent, sub.Plan.NoSurgeFee);

        return Result<UserSubscriptionDto?>.Success(dto);
    }
}
