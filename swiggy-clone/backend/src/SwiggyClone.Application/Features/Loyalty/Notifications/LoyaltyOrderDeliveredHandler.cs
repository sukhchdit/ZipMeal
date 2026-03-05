using MediatR;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Features.Loyalty.Commands.EarnPoints;
using SwiggyClone.Application.Features.Orders.Notifications;

namespace SwiggyClone.Application.Features.Loyalty.Notifications;

internal sealed class LoyaltyOrderDeliveredHandler(
    ISender sender,
    ILogger<LoyaltyOrderDeliveredHandler> logger)
    : INotificationHandler<OrderStatusChangedNotification>
{
    public async Task Handle(OrderStatusChangedNotification notification, CancellationToken ct)
    {
        if (notification.NewStatus != "Delivered")
        {
            return;
        }

        try
        {
            await sender.Send(
                new EarnPointsCommand(notification.UserId, notification.OrderId), ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Failed to earn loyalty points for order {OrderId}, user {UserId}",
                notification.OrderId,
                notification.UserId);
        }
    }
}
