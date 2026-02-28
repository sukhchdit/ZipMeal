using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Infrastructure.Persistence;

namespace SwiggyClone.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class AbandonedOrderCleanupJob(
    AppDbContext dbContext,
    IOptions<BackgroundJobOptions> options,
    ILogger<AbandonedOrderCleanupJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var cutoff = DateTimeOffset.UtcNow.AddMinutes(-options.Value.AbandonedOrderMinutes);
        var now = DateTimeOffset.UtcNow;
        var ct = context.CancellationToken;

        var abandonedOrderIds = await dbContext.Orders
            .Where(o => o.Status == OrderStatus.Placed && o.CreatedAt < cutoff)
            .Select(o => o.Id)
            .ToListAsync(ct);

        if (abandonedOrderIds.Count == 0)
            return;

        await dbContext.Orders
            .Where(o => abandonedOrderIds.Contains(o.Id))
            .ExecuteUpdateAsync(
                s => s
                    .SetProperty(o => o.Status, OrderStatus.Cancelled)
                    .SetProperty(o => o.CancellationReason, "Automatically cancelled — order abandoned")
                    .SetProperty(o => o.UpdatedAt, now),
                ct);

        var historyEntries = abandonedOrderIds.Select(orderId => new OrderStatusHistory
        {
            Id = Guid.CreateVersion7(),
            OrderId = orderId,
            Status = OrderStatus.Cancelled,
            Notes = "Automatically cancelled — order abandoned",
            ChangedBy = null,
            CreatedAt = now
        });

        dbContext.OrderStatusHistory.AddRange(historyEntries);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Cancelled {Count} abandoned orders", abandonedOrderIds.Count);
    }
}
