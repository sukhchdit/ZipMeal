using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Loyalty.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Loyalty.Queries.GetDashboard;

internal sealed class GetLoyaltyDashboardQueryHandler(IAppDbContext db)
    : IRequestHandler<GetLoyaltyDashboardQuery, Result<LoyaltyDashboardDto>>
{
    public async Task<Result<LoyaltyDashboardDto>> Handle(
        GetLoyaltyDashboardQuery request, CancellationToken ct)
    {
        var account = await db.LoyaltyAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.UserId == request.UserId, ct);

        var tiers = await db.LoyaltyTiers
            .AsNoTracking()
            .OrderBy(t => t.Level)
            .ToListAsync(ct);

        var pointsBalance = account?.PointsBalance ?? 0;
        var lifetimeEarned = account?.LifetimePointsEarned ?? 0;
        var currentTierLevel = account?.CurrentTier ?? LoyaltyTierLevel.Bronze;

        var currentTierEntity = tiers.FirstOrDefault(t => t.Level == currentTierLevel)
                                ?? tiers.First();

        var currentTierDto = new LoyaltyTierDto(
            (short)currentTierEntity.Level,
            currentTierEntity.Name,
            currentTierEntity.MinLifetimePoints,
            currentTierEntity.Multiplier);

        // Find next tier
        LoyaltyTierDto? nextTierDto = null;
        var pointsToNext = 0;

        var nextTierEntity = tiers
            .Where(t => t.Level > currentTierLevel)
            .OrderBy(t => t.Level)
            .FirstOrDefault();

        if (nextTierEntity is not null)
        {
            nextTierDto = new LoyaltyTierDto(
                (short)nextTierEntity.Level,
                nextTierEntity.Name,
                nextTierEntity.MinLifetimePoints,
                nextTierEntity.Multiplier);

            pointsToNext = Math.Max(0, nextTierEntity.MinLifetimePoints - lifetimeEarned);
        }

        // Recent 5 transactions
        var recentTxns = account is not null
            ? await db.LoyaltyTransactions
                .AsNoTracking()
                .Where(t => t.LoyaltyAccountId == account.Id)
                .OrderByDescending(t => t.CreatedAt)
                .Take(5)
                .Select(t => new LoyaltyTransactionDto(
                    t.Id, t.Points, (short)t.Type, (short)t.Source,
                    t.ReferenceId, t.Description, t.BalanceAfter, t.CreatedAt))
                .ToListAsync(ct)
            : [];

        var dto = new LoyaltyDashboardDto(
            pointsBalance,
            lifetimeEarned,
            currentTierDto,
            nextTierDto,
            pointsToNext,
            recentTxns);

        return Result<LoyaltyDashboardDto>.Success(dto);
    }
}
