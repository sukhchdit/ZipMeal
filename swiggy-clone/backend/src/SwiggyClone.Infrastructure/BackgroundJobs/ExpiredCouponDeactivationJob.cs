using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using SwiggyClone.Infrastructure.Persistence;

namespace SwiggyClone.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class ExpiredCouponDeactivationJob(
    AppDbContext dbContext,
    ILogger<ExpiredCouponDeactivationJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var now = DateTimeOffset.UtcNow;

        var deactivated = await dbContext.Coupons
            .Where(c => c.IsActive && c.ValidUntil < now)
            .ExecuteUpdateAsync(
                s => s
                    .SetProperty(c => c.IsActive, false)
                    .SetProperty(c => c.UpdatedAt, now),
                context.CancellationToken);

        if (deactivated > 0)
        {
            logger.LogInformation("Deactivated {Count} expired coupons", deactivated);
        }
    }
}
