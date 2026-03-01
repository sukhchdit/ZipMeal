using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using SwiggyClone.Infrastructure.Persistence;

namespace SwiggyClone.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class ExpiredPromotionDeactivationJob(
    AppDbContext dbContext,
    ILogger<ExpiredPromotionDeactivationJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var now = DateTimeOffset.UtcNow;
        var deactivated = await dbContext.RestaurantPromotions
            .Where(p => p.IsActive && p.ValidUntil < now)
            .ExecuteUpdateAsync(
                s => s
                    .SetProperty(p => p.IsActive, false)
                    .SetProperty(p => p.UpdatedAt, now),
                context.CancellationToken);

        if (deactivated > 0)
            logger.LogInformation("Deactivated {Count} expired promotions", deactivated);
    }
}
