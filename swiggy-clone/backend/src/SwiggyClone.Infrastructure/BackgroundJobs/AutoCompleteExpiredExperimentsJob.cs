using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Infrastructure.Persistence;

namespace SwiggyClone.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class AutoCompleteExpiredExperimentsJob(
    AppDbContext dbContext,
    ILogger<AutoCompleteExpiredExperimentsJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var now = DateTimeOffset.UtcNow;
        var ct = context.CancellationToken;

        var completedCount = await dbContext.Experiments
            .Where(e => e.Status == ExperimentStatus.Active
                && e.EndDate != null
                && e.EndDate <= now)
            .ExecuteUpdateAsync(
                s => s
                    .SetProperty(e => e.Status, ExperimentStatus.Completed)
                    .SetProperty(e => e.UpdatedAt, now),
                ct);

        if (completedCount > 0)
        {
            logger.LogInformation("Auto-completed {Count} expired experiments", completedCount);
        }
    }
}
