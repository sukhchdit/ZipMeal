using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Infrastructure.Persistence;

namespace SwiggyClone.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class StalledPaymentJob(
    AppDbContext dbContext,
    IOptions<BackgroundJobOptions> options,
    ILogger<StalledPaymentJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var cutoff = DateTimeOffset.UtcNow.AddMinutes(-options.Value.StalledPaymentMinutes);
        var now = DateTimeOffset.UtcNow;

        var failed = await dbContext.Payments
            .Where(p => p.Status == PaymentStatus.Pending && p.CreatedAt < cutoff)
            .ExecuteUpdateAsync(
                s => s
                    .SetProperty(p => p.Status, PaymentStatus.Failed)
                    .SetProperty(p => p.UpdatedAt, now),
                context.CancellationToken);

        if (failed > 0)
        {
            logger.LogInformation("Marked {Count} stalled payments as failed", failed);
        }
    }
}
