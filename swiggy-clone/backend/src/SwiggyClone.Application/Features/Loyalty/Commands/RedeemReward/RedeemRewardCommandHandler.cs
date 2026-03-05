using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Loyalty.Commands.GetOrCreateLoyaltyAccount;
using SwiggyClone.Application.Features.Loyalty.DTOs;
using SwiggyClone.Application.Features.Wallet.Commands.CreditWallet;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Application.Features.Loyalty.Commands.RedeemReward;

internal sealed class RedeemRewardCommandHandler(
    IAppDbContext db,
    ISender sender,
    IEventBus eventBus,
    ILogger<RedeemRewardCommandHandler> logger)
    : IRequestHandler<RedeemRewardCommand, Result<LoyaltyTransactionDto>>
{
    public async Task<Result<LoyaltyTransactionDto>> Handle(
        RedeemRewardCommand request, CancellationToken ct)
    {
        // 1. GetOrCreate account
        var account = await sender.Send(new GetOrCreateLoyaltyAccountCommand(request.UserId), ct);

        // 2. Load reward
        var reward = await db.LoyaltyRewards
            .FirstOrDefaultAsync(r => r.Id == request.RewardId, ct);

        if (reward is null)
        {
            return Result<LoyaltyTransactionDto>.Failure(
                "LOYALTY_REWARD_NOT_FOUND", "Reward not found.");
        }

        if (!reward.IsRedeemable())
        {
            return Result<LoyaltyTransactionDto>.Failure(
                "LOYALTY_REWARD_UNAVAILABLE", "This reward is not available for redemption.");
        }

        if (account.PointsBalance < reward.PointsCost)
        {
            return Result<LoyaltyTransactionDto>.Failure(
                "LOYALTY_INSUFFICIENT_POINTS", "Insufficient loyalty points.");
        }

        if (reward.Stock is not null && reward.Stock <= 0)
        {
            return Result<LoyaltyTransactionDto>.Failure(
                "LOYALTY_REWARD_OUT_OF_STOCK", "This reward is out of stock.");
        }

        // 3. Deduct points
        account.RedeemPoints(reward.PointsCost);

        // 4. Decrement stock
        reward.DecrementStock();

        // 5. Create transaction
        var txn = LoyaltyTransaction.Create(
            account.Id,
            reward.PointsCost,
            LoyaltyTransactionType.Redeem,
            LoyaltyTransactionSource.Redemption,
            reward.Id,
            $"Redeemed: {reward.Name}",
            account.PointsBalance);

        db.LoyaltyTransactions.Add(txn);

        // 6. Credit wallet if WalletCredit type
        if (reward.RewardType == LoyaltyRewardType.WalletCredit)
        {
            await sender.Send(new CreditWalletCommand(
                request.UserId,
                reward.RewardValue,
                (short)WalletTransactionSource.LoyaltyRedemption,
                reward.Id,
                $"Loyalty reward: {reward.Name}"), ct);
        }

        // 7. Publish Kafka event
        try
        {
            await eventBus.PublishAsync(
                KafkaTopics.LoyaltyRewardRedeemed,
                account.Id.ToString(),
                new
                {
                    account.UserId,
                    RewardId = reward.Id,
                    reward.Name,
                    reward.PointsCost,
                    NewBalance = account.PointsBalance,
                    Timestamp = DateTimeOffset.UtcNow,
                }, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish LoyaltyRewardRedeemed event for reward {RewardId}", reward.Id);
        }

        await db.SaveChangesAsync(ct);

        var dto = new LoyaltyTransactionDto(
            txn.Id, txn.Points, (short)txn.Type, (short)txn.Source,
            txn.ReferenceId, txn.Description, txn.BalanceAfter, txn.CreatedAt);

        return Result<LoyaltyTransactionDto>.Success(dto);
    }
}
