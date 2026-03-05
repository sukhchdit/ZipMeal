using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Infrastructure.Persistence;

namespace SwiggyClone.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class ExpireGroupOrdersJob(
    AppDbContext dbContext,
    ILogger<ExpireGroupOrdersJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var now = DateTimeOffset.UtcNow;
        var ct = context.CancellationToken;

        var expiredCount = await dbContext.GroupOrders
            .Where(g => g.Status == GroupOrderStatus.Active && g.ExpiresAt <= now)
            .ExecuteUpdateAsync(
                s => s
                    .SetProperty(g => g.Status, GroupOrderStatus.Expired)
                    .SetProperty(g => g.UpdatedAt, now),
                ct);

        if (expiredCount > 0)
        {
            logger.LogInformation("Expired {Count} stale group orders", expiredCount);
        }
    }
}
