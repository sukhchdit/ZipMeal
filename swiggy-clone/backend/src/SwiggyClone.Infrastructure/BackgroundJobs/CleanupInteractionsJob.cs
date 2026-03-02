using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using SwiggyClone.Infrastructure.Persistence;

namespace SwiggyClone.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class CleanupInteractionsJob(
    AppDbContext dbContext,
    IOptions<BackgroundJobOptions> options,
    ILogger<CleanupInteractionsJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var retentionDays = options.Value.InteractionRetentionDays;
        var cutoff = DateTimeOffset.UtcNow.AddDays(-retentionDays);

        var deletedCount = await dbContext.UserInteractions
            .Where(i => i.CreatedAt < cutoff)
            .ExecuteDeleteAsync(context.CancellationToken);

        if (deletedCount > 0)
        {
            logger.LogInformation(
                "Cleaned up {Count} user interactions older than {Days} days",
                deletedCount,
                retentionDays);
        }
    }
}
