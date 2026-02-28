using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Payments.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Payments.Commands;

internal sealed class VerifyPaymentCommandHandler(
    IAppDbContext db,
    IPaymentGatewayService gateway)
    : IRequestHandler<VerifyPaymentCommand, Result<PaymentDto>>
{
    public async Task<Result<PaymentDto>> Handle(
        VerifyPaymentCommand request, CancellationToken ct)
    {
        var payment = await db.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.GatewayOrderId == request.GatewayOrderId, ct);

        if (payment is null)
            return Result<PaymentDto>.Failure("PAYMENT_NOT_FOUND", "Payment not found.");

        if (payment.Order.UserId != request.UserId)
            return Result<PaymentDto>.Failure("UNAUTHORIZED", "You are not authorized to verify this payment.");

        // Idempotent: already paid
        if (payment.Status == PaymentStatus.Paid)
            return Result<PaymentDto>.Success(MapToDto(payment));

        var verifyResult = await gateway.VerifyPaymentAsync(
            request.GatewayOrderId, request.GatewayPaymentId, request.GatewaySignature, ct);

        if (verifyResult.IsValid)
        {
            payment.Status = PaymentStatus.Paid;
            payment.GatewayPaymentId = request.GatewayPaymentId;
            payment.GatewaySignature = request.GatewaySignature;
            payment.Method = verifyResult.PaymentMethod;
            payment.UpdatedAt = DateTimeOffset.UtcNow;

            payment.Order.PaymentStatus = PaymentStatus.Paid;

            // If this is a dine-in order, bulk-update all session orders
            if (payment.Order.DineInSessionId is not null)
            {
                await CompleteDineInSessionPayment(payment.Order.DineInSessionId.Value, ct);
            }
        }
        else
        {
            payment.Status = PaymentStatus.Failed;
            payment.UpdatedAt = DateTimeOffset.UtcNow;

            payment.Order.PaymentStatus = PaymentStatus.Failed;
        }

        await db.SaveChangesAsync(ct);

        return Result<PaymentDto>.Success(MapToDto(payment));
    }

    private async Task CompleteDineInSessionPayment(Guid sessionId, CancellationToken ct)
    {
        // Mark all session orders' payments as Paid
        var sessionOrders = await db.Orders
            .Where(o => o.DineInSessionId == sessionId)
            .ToListAsync(ct);

        foreach (var order in sessionOrders)
        {
            order.PaymentStatus = PaymentStatus.Paid;
        }

        var sessionPayments = await db.Payments
            .Where(p => sessionOrders.Select(o => o.Id).Contains(p.OrderId))
            .ToListAsync(ct);

        foreach (var p in sessionPayments)
        {
            if (p.Status != PaymentStatus.Paid)
            {
                p.Status = PaymentStatus.Paid;
                p.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }

        // Complete session and free table
        var session = await db.DineInSessions
            .Include(s => s.Table)
            .FirstOrDefaultAsync(s => s.Id == sessionId, ct);

        if (session is not null)
        {
            session.Status = DineInSessionStatus.Completed;
            session.EndedAt = DateTimeOffset.UtcNow;
            session.UpdatedAt = DateTimeOffset.UtcNow;
            session.Table.Status = TableStatus.Available;
            session.Table.UpdatedAt = DateTimeOffset.UtcNow;
        }
    }

    private static PaymentDto MapToDto(Domain.Entities.Payment payment) =>
        new(
            payment.Id,
            payment.OrderId,
            payment.PaymentGateway,
            payment.GatewayPaymentId,
            payment.GatewayOrderId,
            payment.Amount,
            payment.Currency,
            payment.Status,
            payment.Method,
            payment.RefundAmount,
            payment.RefundReason,
            payment.RefundedAt,
            payment.CreatedAt);
}
