using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Infrastructure.Persistence;

namespace SwiggyClone.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class LoyaltyPointsExpiryJob(
    AppDbContext dbContext,
    IOptions<BackgroundJobOptions> options,
    ILogger<LoyaltyPointsExpiryJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var inactivityDays = options.Value.LoyaltyInactivityDays;
        var cutoff = DateTimeOffset.UtcNow.AddDays(-inactivityDays);

        var expiredAccounts = await dbContext.LoyaltyAccounts
            .Where(a => a.PointsBalance > 0 && a.LastActivityAt < cutoff)
            .ToListAsync(context.CancellationToken);

        if (expiredAccounts.Count == 0)
        {
            return;
        }

        foreach (var account in expiredAccounts)
        {
            var expiredPoints = account.PointsBalance;
            account.ExpirePoints();

            var txn = LoyaltyTransaction.Create(
                account.Id,
                expiredPoints,
                LoyaltyTransactionType.Expire,
                LoyaltyTransactionSource.Expiry,
                null,
                $"Points expired due to {inactivityDays} days of inactivity",
                account.PointsBalance);

            dbContext.LoyaltyTransactions.Add(txn);
        }

        await dbContext.SaveChangesAsync(context.CancellationToken);

        logger.LogInformation(
            "Expired loyalty points for {Count} accounts (inactivity > {Days} days)",
            expiredAccounts.Count,
            inactivityDays);
    }
}
