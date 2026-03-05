using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Infrastructure.Persistence;

namespace SwiggyClone.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class EscalateDisputesJob(
    AppDbContext dbContext,
    IOptions<BackgroundJobOptions> options,
    ILogger<EscalateDisputesJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var now = DateTimeOffset.UtcNow;
        var ct = context.CancellationToken;
        var escalationHours = options.Value.DisputeEscalationHours;
        var threshold = now.AddHours(-escalationHours);

        var escalatedCount = await dbContext.Disputes
            .Where(d => (d.Status == DisputeStatus.Opened || d.Status == DisputeStatus.UnderReview)
                && d.CreatedAt <= threshold)
            .ExecuteUpdateAsync(
                s => s
                    .SetProperty(d => d.Status, DisputeStatus.Escalated)
                    .SetProperty(d => d.EscalatedAt, now)
                    .SetProperty(d => d.UpdatedAt, now),
                ct);

        if (escalatedCount > 0)
        {
            logger.LogInformation("Escalated {Count} stale disputes (older than {Hours}h)", escalatedCount, escalationHours);
        }
    }
}
