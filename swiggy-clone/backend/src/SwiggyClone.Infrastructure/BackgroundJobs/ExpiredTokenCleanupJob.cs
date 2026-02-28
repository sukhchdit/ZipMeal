using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using SwiggyClone.Infrastructure.Persistence;

namespace SwiggyClone.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class ExpiredTokenCleanupJob(
    AppDbContext dbContext,
    IOptions<BackgroundJobOptions> options,
    ILogger<ExpiredTokenCleanupJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var cutoff = DateTimeOffset.UtcNow.AddDays(-options.Value.TokenRetentionDays);

        var deleted = await dbContext.RefreshTokens
            .Where(t => t.ExpiresAt < cutoff || t.RevokedAt < cutoff)
            .ExecuteDeleteAsync(context.CancellationToken);

        if (deleted > 0)
        {
            logger.LogInformation("Purged {Count} expired/revoked refresh tokens", deleted);
        }
    }
}
