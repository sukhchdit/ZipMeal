using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Loyalty.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Loyalty.Queries.GetRewards;

internal sealed class GetLoyaltyRewardsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetLoyaltyRewardsQuery, Result<IReadOnlyList<LoyaltyRewardDto>>>
{
    public async Task<Result<IReadOnlyList<LoyaltyRewardDto>>> Handle(
        GetLoyaltyRewardsQuery request, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;

        var rewards = await db.LoyaltyRewards
            .AsNoTracking()
            .Where(r => r.IsActive)
            .Where(r => !r.ExpiresAt.HasValue || r.ExpiresAt > now)
            .OrderBy(r => r.PointsCost)
            .Select(r => new LoyaltyRewardDto(
                r.Id, r.Name, r.Description, r.PointsCost,
                (short)r.RewardType, r.RewardValue, r.Stock, r.ExpiresAt))
            .ToListAsync(ct);

        return Result<IReadOnlyList<LoyaltyRewardDto>>.Success(rewards);
    }
}
