using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using SwiggyClone.Application.Features.Orders.Notifications;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Infrastructure.Persistence;

namespace SwiggyClone.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class ScheduledOrderActivationJob(
    AppDbContext dbContext,
    IPublisher publisher,
    ILogger<ScheduledOrderActivationJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var now = DateTimeOffset.UtcNow;
        var ct = context.CancellationToken;

        var readyOrders = await dbContext.Orders
            .Where(o => o.Status == OrderStatus.Scheduled
                && o.ScheduledDeliveryTime != null
                && o.ScheduledDeliveryTime <= now)
            .Select(o => new { o.Id, o.UserId, o.RestaurantId, o.OrderNumber, o.TotalAmount })
            .ToListAsync(ct);

        if (readyOrders.Count == 0) return;

        var readyIds = readyOrders.Select(o => o.Id).ToList();

        await dbContext.Orders
            .Where(o => readyIds.Contains(o.Id))
            .ExecuteUpdateAsync(s => s
                .SetProperty(o => o.Status, OrderStatus.Placed)
                .SetProperty(o => o.UpdatedAt, now), ct);

        dbContext.OrderStatusHistory.AddRange(readyOrders.Select(o => new OrderStatusHistory
        {
            Id = Guid.CreateVersion7(),
            OrderId = o.Id,
            Status = OrderStatus.Placed,
            Notes = "Scheduled order activated",
            ChangedBy = null,
            CreatedAt = now
        }));
        await dbContext.SaveChangesAsync(ct);

        foreach (var order in readyOrders)
        {
            await publisher.Publish(new OrderPlacedNotification(
                order.Id, order.UserId, order.RestaurantId,
                order.OrderNumber, order.TotalAmount), ct);
        }

        logger.LogInformation("Activated {Count} scheduled orders", readyOrders.Count);
    }
}
