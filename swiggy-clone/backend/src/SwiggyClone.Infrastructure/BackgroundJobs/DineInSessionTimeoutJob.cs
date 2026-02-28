using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Infrastructure.Persistence;

namespace SwiggyClone.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class DineInSessionTimeoutJob(
    AppDbContext dbContext,
    IOptions<BackgroundJobOptions> options,
    ILogger<DineInSessionTimeoutJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var cutoff = DateTimeOffset.UtcNow.AddMinutes(-options.Value.DineInPaymentPendingMinutes);
        var now = DateTimeOffset.UtcNow;
        var ct = context.CancellationToken;

        var stuckSessions = await dbContext.DineInSessions
            .Where(s => s.Status == DineInSessionStatus.PaymentPending && s.UpdatedAt < cutoff)
            .Select(s => new { s.Id, s.TableId })
            .ToListAsync(ct);

        if (stuckSessions.Count == 0)
            return;

        var sessionIds = stuckSessions.Select(s => s.Id).ToList();
        var tableIds = stuckSessions.Select(s => s.TableId).Distinct().ToList();

        await dbContext.DineInSessions
            .Where(s => sessionIds.Contains(s.Id))
            .ExecuteUpdateAsync(
                s => s
                    .SetProperty(x => x.Status, DineInSessionStatus.Cancelled)
                    .SetProperty(x => x.EndedAt, now)
                    .SetProperty(x => x.UpdatedAt, now),
                ct);

        await dbContext.RestaurantTables
            .Where(t => tableIds.Contains(t.Id) && t.Status == TableStatus.Occupied)
            .ExecuteUpdateAsync(
                s => s
                    .SetProperty(t => t.Status, TableStatus.Available)
                    .SetProperty(t => t.UpdatedAt, now),
                ct);

        logger.LogInformation("Timed out {Count} stuck dine-in sessions", stuckSessions.Count);
    }
}
