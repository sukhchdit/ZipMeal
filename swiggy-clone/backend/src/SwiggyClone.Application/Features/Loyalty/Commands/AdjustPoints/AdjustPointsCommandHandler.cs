using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Loyalty.Commands.GetOrCreateLoyaltyAccount;
using SwiggyClone.Application.Features.Loyalty.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Loyalty.Commands.AdjustPoints;

internal sealed class AdjustPointsCommandHandler(IAppDbContext db, ISender sender)
    : IRequestHandler<AdjustPointsCommand, Result<LoyaltyTransactionDto>>
{
    public async Task<Result<LoyaltyTransactionDto>> Handle(
        AdjustPointsCommand request, CancellationToken ct)
    {
        var account = await sender.Send(new GetOrCreateLoyaltyAccountCommand(request.UserId), ct);

        if (request.Points > 0)
        {
            account.EarnPoints(request.Points);
        }
        else
        {
            var absPoints = Math.Abs(request.Points);
            if (account.PointsBalance < absPoints)
            {
                return Result<LoyaltyTransactionDto>.Failure(
                    "LOYALTY_INSUFFICIENT_POINTS",
                    $"Cannot deduct {absPoints} points. Current balance: {account.PointsBalance}.");
            }

            account.RedeemPoints(absPoints);
        }

        // Recalculate tier on positive adjustment
        if (request.Points > 0)
        {
            var tiers = await db.LoyaltyTiers.AsNoTracking().ToListAsync(ct);
            account.RecalculateTier(tiers);
        }

        var txn = LoyaltyTransaction.Create(
            account.Id,
            Math.Abs(request.Points),
            request.Points > 0 ? LoyaltyTransactionType.Earn : LoyaltyTransactionType.Redeem,
            LoyaltyTransactionSource.AdminAward,
            null,
            request.Description,
            account.PointsBalance);

        db.LoyaltyTransactions.Add(txn);
        await db.SaveChangesAsync(ct);

        var dto = new LoyaltyTransactionDto(
            txn.Id, txn.Points, (short)txn.Type, (short)txn.Source,
            txn.ReferenceId, txn.Description, txn.BalanceAfter, txn.CreatedAt);

        return Result<LoyaltyTransactionDto>.Success(dto);
    }
}
