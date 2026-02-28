using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Payments.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Payments.Commands;

internal sealed class PayDineInSessionCommandHandler(
    IAppDbContext db,
    IPaymentGatewayService gateway)
    : IRequestHandler<PayDineInSessionCommand, Result<CreatePaymentOrderResponseDto>>
{
    public async Task<Result<CreatePaymentOrderResponseDto>> Handle(
        PayDineInSessionCommand request, CancellationToken ct)
    {
        var session = await db.DineInSessions
            .Include(s => s.Members)
            .FirstOrDefaultAsync(s => s.Id == request.SessionId, ct);

        if (session is null)
            return Result<CreatePaymentOrderResponseDto>.Failure(
                "SESSION_NOT_FOUND", "Dine-in session not found.");

        // Only host can pay
        var hostMember = session.Members
            .FirstOrDefault(m => m.UserId == request.UserId && m.Role == 1);

        if (hostMember is null)
            return Result<CreatePaymentOrderResponseDto>.Failure(
                "UNAUTHORIZED", "Only the session host can initiate payment.");

        if (session.Status is not (DineInSessionStatus.BillRequested or DineInSessionStatus.PaymentPending))
            return Result<CreatePaymentOrderResponseDto>.Failure(
                "INVALID_SESSION_STATUS",
                "Payment can only be initiated when the bill has been requested.");

        // Get all session orders and sum total
        var sessionOrders = await db.Orders
            .Where(o => o.DineInSessionId == session.Id)
            .ToListAsync(ct);

        if (sessionOrders.Count == 0)
            return Result<CreatePaymentOrderResponseDto>.Failure(
                "NO_ORDERS", "No orders found for this session.");

        var totalAmount = sessionOrders.Sum(o => o.TotalAmount);

        // Check if gateway order already created (idempotent)
        var firstOrder = sessionOrders[0];
        var existingPayment = await db.Payments
            .FirstOrDefaultAsync(p => p.OrderId == firstOrder.Id, ct);

        if (existingPayment?.GatewayOrderId is not null)
        {
            return Result<CreatePaymentOrderResponseDto>.Success(
                new CreatePaymentOrderResponseDto(
                    existingPayment.Id, existingPayment.GatewayOrderId,
                    totalAmount, "INR", existingPayment.PaymentGateway));
        }

        // Create gateway order for the session total
        var receiptId = $"DINE-{session.SessionCode}";
        var gatewayResult = await gateway.CreateOrderAsync(totalAmount, "INR", receiptId, ct);

        if (!gatewayResult.Success)
            return Result<CreatePaymentOrderResponseDto>.Failure(
                "GATEWAY_ERROR", gatewayResult.Error ?? "Failed to create payment order.");

        // Update session status
        session.Status = DineInSessionStatus.PaymentPending;
        session.UpdatedAt = DateTimeOffset.UtcNow;

        // Update first order's payment with gateway details
        if (existingPayment is not null)
        {
            existingPayment.GatewayOrderId = gatewayResult.GatewayOrderId;
            existingPayment.PaymentGateway = "DevGateway";
            existingPayment.Amount = totalAmount;
            existingPayment.UpdatedAt = DateTimeOffset.UtcNow;
        }

        // Update payment method on all session orders
        var paymentMethod = (PaymentMethod)request.PaymentMethod;
        foreach (var order in sessionOrders)
        {
            order.PaymentMethod = paymentMethod;
        }

        await db.SaveChangesAsync(ct);

        return Result<CreatePaymentOrderResponseDto>.Success(
            new CreatePaymentOrderResponseDto(
                existingPayment?.Id ?? Guid.Empty,
                gatewayResult.GatewayOrderId!,
                totalAmount, "INR", "DevGateway"));
    }
}
