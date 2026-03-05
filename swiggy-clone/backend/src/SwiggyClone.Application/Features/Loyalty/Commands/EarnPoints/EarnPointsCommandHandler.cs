using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Loyalty.Commands.GetOrCreateLoyaltyAccount;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Application.Features.Loyalty.Commands.EarnPoints;

internal sealed class EarnPointsCommandHandler(
    IAppDbContext db,
    ISender sender,
    IEventBus eventBus,
    ILogger<EarnPointsCommandHandler> logger)
    : IRequestHandler<EarnPointsCommand, Result<int>>
{
    public async Task<Result<int>> Handle(EarnPointsCommand request, CancellationToken ct)
    {
        // 1. Guard: idempotency check
        var alreadyEarned = await db.LoyaltyTransactions
            .AnyAsync(t => t.ReferenceId == request.OrderId
                           && t.Source == LoyaltyTransactionSource.OrderDelivered, ct);

        if (alreadyEarned)
        {
            return Result<int>.Failure("LOYALTY_ALREADY_EARNED", "Points already earned for this order.");
        }

        // 2. Load order
        var order = await db.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        if (order is null)
        {
            return Result<int>.Failure("ORDER_NOT_FOUND", "Order not found.");
        }

        // 3. Calculate eligible amount (paise)
        var eligiblePaise = order.TotalAmount - order.DeliveryFee;
        if (eligiblePaise <= 0)
        {
            return Result<int>.Success(0);
        }

        // 4. GetOrCreate loyalty account
        var account = await sender.Send(new GetOrCreateLoyaltyAccountCommand(request.UserId), ct);

        // 5. Load tier config
        var tiers = await db.LoyaltyTiers.AsNoTracking().ToListAsync(ct);
        var currentTier = tiers.FirstOrDefault(t => t.Level == account.CurrentTier)
                          ?? tiers.First(t => t.Level == LoyaltyTierLevel.Bronze);

        // 6. Base points: 1 point per ₹100 (10000 paise)
        var basePoints = eligiblePaise / 10000 * currentTier.PointsPerHundredPaise;
        if (basePoints <= 0)
        {
            basePoints = 1; // Minimum 1 point per delivered order
        }

        // 7. Apply tier multiplier
        var points = (int)(basePoints * currentTier.Multiplier);

        // 8. Check active subscription → +50% bonus
        var hasActiveSubscription = await db.UserSubscriptions
            .AnyAsync(s => s.UserId == request.UserId
                           && s.Status == SubscriptionStatus.Active
                           && s.EndDate > DateTimeOffset.UtcNow, ct);

        if (hasActiveSubscription)
        {
            points = (int)(points * 1.5);
        }

        if (points <= 0)
        {
            points = 1;
        }

        // 9. Earn points
        account.EarnPoints(points);

        // 10. Recalculate tier
        account.RecalculateTier(tiers);

        // 11. Create transaction
        var txn = LoyaltyTransaction.Create(
            account.Id,
            points,
            LoyaltyTransactionType.Earn,
            LoyaltyTransactionSource.OrderDelivered,
            request.OrderId,
            $"Earned {points} points for order delivery",
            account.PointsBalance);

        db.LoyaltyTransactions.Add(txn);

        // 12. Publish Kafka event
        try
        {
            await eventBus.PublishAsync(
                KafkaTopics.LoyaltyPointsEarned,
                account.Id.ToString(),
                new
                {
                    account.UserId,
                    request.OrderId,
                    Points = points,
                    NewBalance = account.PointsBalance,
                    Tier = account.CurrentTier.ToString(),
                    Timestamp = DateTimeOffset.UtcNow,
                }, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish LoyaltyPointsEarned event for order {OrderId}", request.OrderId);
        }

        await db.SaveChangesAsync(ct);

        return Result<int>.Success(points);
    }
}
