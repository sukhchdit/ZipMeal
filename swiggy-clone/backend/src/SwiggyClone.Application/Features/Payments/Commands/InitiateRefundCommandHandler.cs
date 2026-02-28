using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Payments.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Payments.Commands;

internal sealed class InitiateRefundCommandHandler(
    IAppDbContext db,
    IPaymentGatewayService gateway)
    : IRequestHandler<InitiateRefundCommand, Result<PaymentDto>>
{
    public async Task<Result<PaymentDto>> Handle(
        InitiateRefundCommand request, CancellationToken ct)
    {
        var payment = await db.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.OrderId == request.OrderId, ct);

        if (payment is null)
            return Result<PaymentDto>.Failure("PAYMENT_NOT_FOUND", "Payment not found.");

        if (payment.Status != PaymentStatus.Paid)
            return Result<PaymentDto>.Failure(
                "REFUND_NOT_ALLOWED", "Only paid payments can be refunded.");

        if (payment.GatewayPaymentId is null)
            return Result<PaymentDto>.Failure(
                "REFUND_NOT_ALLOWED", "No gateway payment ID found for refund.");

        // Idempotent: already refunded
        if (payment.Status == PaymentStatus.Refunded)
        {
            return Result<PaymentDto>.Success(new PaymentDto(
                payment.Id, payment.OrderId, payment.PaymentGateway,
                payment.GatewayPaymentId, payment.GatewayOrderId,
                payment.Amount, payment.Currency, payment.Status,
                payment.Method, payment.RefundAmount, payment.RefundReason,
                payment.RefundedAt, payment.CreatedAt));
        }

        var refundResult = await gateway.RefundAsync(
            payment.GatewayPaymentId, payment.Amount, ct);

        if (!refundResult.Success)
            return Result<PaymentDto>.Failure(
                "REFUND_FAILED", refundResult.Error ?? "Failed to process refund.");

        payment.Status = PaymentStatus.Refunded;
        payment.RefundAmount = payment.Amount;
        payment.RefundReason = request.Reason ?? "Refund initiated.";
        payment.RefundedAt = DateTimeOffset.UtcNow;
        payment.UpdatedAt = DateTimeOffset.UtcNow;

        payment.Order.PaymentStatus = PaymentStatus.Refunded;

        await db.SaveChangesAsync(ct);

        return Result<PaymentDto>.Success(new PaymentDto(
            payment.Id, payment.OrderId, payment.PaymentGateway,
            payment.GatewayPaymentId, payment.GatewayOrderId,
            payment.Amount, payment.Currency, payment.Status,
            payment.Method, payment.RefundAmount, payment.RefundReason,
            payment.RefundedAt, payment.CreatedAt));
    }
}
