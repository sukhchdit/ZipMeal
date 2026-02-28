using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Referrals.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Referrals.Queries.GetReferralStats;

internal sealed class GetReferralStatsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetReferralStatsQuery, Result<ReferralStatsDto>>
{
    public async Task<Result<ReferralStatsDto>> Handle(
        GetReferralStatsQuery request, CancellationToken ct)
    {
        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.UserId, ct);

        if (user is null)
            return Result<ReferralStatsDto>.Failure("USER_NOT_FOUND", "User not found.");

        var totalReferrals = await db.Users
            .CountAsync(u => u.ReferredByUserId == request.UserId, ct);

        var totalRewardsPaise = await db.WalletTransactions
            .Where(wt => wt.Wallet.UserId == request.UserId
                && wt.Source == WalletTransactionSource.Referral)
            .SumAsync(wt => wt.AmountPaise, ct);

        return Result<ReferralStatsDto>.Success(
            new ReferralStatsDto(user.ReferralCode, totalReferrals, totalRewardsPaise));
    }
}
