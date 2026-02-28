using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Subscriptions.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Subscriptions.Commands.SubscribeToPlan;

internal sealed class SubscribeToPlanCommandHandler(IAppDbContext db)
    : IRequestHandler<SubscribeToPlanCommand, Result<UserSubscriptionDto>>
{
    public async Task<Result<UserSubscriptionDto>> Handle(SubscribeToPlanCommand request, CancellationToken ct)
    {
        // 1. Find plan (must exist and be active)
        var plan = await db.SubscriptionPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PlanId && p.IsActive, ct);

        if (plan is null)
            return Result<UserSubscriptionDto>.Failure("PLAN_NOT_FOUND", "Subscription plan not found or inactive.");

        // 2. Check no existing active subscription
        var hasActive = await db.UserSubscriptions
            .AnyAsync(s => s.UserId == request.UserId
                && s.Status == SubscriptionStatus.Active
                && s.EndDate > DateTimeOffset.UtcNow, ct);

        if (hasActive)
            return Result<UserSubscriptionDto>.Failure("ALREADY_SUBSCRIBED",
                "You already have an active subscription. Cancel it first or wait for it to expire.");

        // 3. Create subscription
        var now = DateTimeOffset.UtcNow;
        var subscription = new UserSubscription
        {
            Id = Guid.CreateVersion7(),
            UserId = request.UserId,
            PlanId = plan.Id,
            StartDate = now,
            EndDate = now.AddDays(plan.DurationDays),
            Status = SubscriptionStatus.Active,
            PaidAmountPaise = plan.PricePaise,
            CreatedAt = now,
        };

        db.UserSubscriptions.Add(subscription);
        await db.SaveChangesAsync(ct);

        // 4. Return DTO with plan benefits
        var dto = new UserSubscriptionDto(
            subscription.Id, plan.Id, plan.Name, subscription.PaidAmountPaise,
            subscription.StartDate, subscription.EndDate, (int)subscription.Status,
            plan.FreeDelivery, plan.ExtraDiscountPercent, plan.NoSurgeFee);

        return Result<UserSubscriptionDto>.Success(dto);
    }
}
