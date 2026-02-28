using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Infrastructure.Persistence;

namespace SwiggyClone.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class SubscriptionExpiryJob(
    AppDbContext dbContext,
    ILogger<SubscriptionExpiryJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var now = DateTimeOffset.UtcNow;

        var expired = await dbContext.UserSubscriptions
            .Where(s => s.Status == SubscriptionStatus.Active && s.EndDate < now)
            .ExecuteUpdateAsync(
                s => s.SetProperty(x => x.Status, SubscriptionStatus.Expired),
                context.CancellationToken);

        if (expired > 0)
        {
            logger.LogInformation("Expired {Count} subscriptions", expired);
        }
    }
}
