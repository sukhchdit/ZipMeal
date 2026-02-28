using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Diagnostics;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Application.Features.Orders.Notifications;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Orders.Commands;

internal sealed class CancelOrderCommandHandler(IAppDbContext db, IPaymentGatewayService paymentGateway, IPublisher publisher)
    : IRequestHandler<CancelOrderCommand, Result>
{
    public async Task<Result> Handle(CancelOrderCommand request, CancellationToken ct)
    {
        var order = await db.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.UserId == request.UserId, ct);

        if (order is null)
            return Result.Failure("ORDER_NOT_FOUND", "Order not found.");

        if (order.Status is not (OrderStatus.Placed or OrderStatus.Confirmed))
            return Result.Failure("CANCELLATION_NOT_ALLOWED",
                "Order can only be cancelled when in Placed or Confirmed status.");

        order.Status = OrderStatus.Cancelled;
        order.CancellationReason = request.CancellationReason;
        order.CancelledBy = 1; // Customer

        db.OrderStatusHistory.Add(new OrderStatusHistory
        {
            Id = Guid.CreateVersion7(),
            OrderId = order.Id,
            Status = OrderStatus.Cancelled,
            Notes = request.CancellationReason ?? "Cancelled by customer.",
            ChangedBy = request.UserId,
            CreatedAt = DateTimeOffset.UtcNow,
        });

        // If the order was prepaid (online payment), initiate refund
        if (order.PaymentStatus == PaymentStatus.Paid)
        {
            var payment = await db.Payments
                .FirstOrDefaultAsync(p => p.OrderId == order.Id, ct);

            if (payment is not null && payment.GatewayPaymentId is not null)
            {
                var refundResult = await paymentGateway.RefundAsync(
                    payment.GatewayPaymentId, payment.Amount, ct);

                if (refundResult.Success)
                {
                    payment.Status = PaymentStatus.Refunded;
                    payment.RefundAmount = payment.Amount;
                    payment.RefundReason = request.CancellationReason ?? "Order cancelled by customer.";
                    payment.RefundedAt = DateTimeOffset.UtcNow;
                    payment.UpdatedAt = DateTimeOffset.UtcNow;
                    order.PaymentStatus = PaymentStatus.Refunded;
                }
            }
        }

        await db.SaveChangesAsync(ct);

        ApplicationDiagnostics.OrdersCancelled.Add(1);

        var refundInitiated = order.PaymentStatus == PaymentStatus.Refunded;
        await publisher.Publish(new OrderCancelledNotification(
            order.Id, order.UserId, order.CancellationReason, refundInitiated), ct);

        return Result.Success();
    }
}
