using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Payments.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Payments.Commands;

internal sealed class CreatePaymentOrderCommandHandler(
    IAppDbContext db,
    IPaymentGatewayService gateway)
    : IRequestHandler<CreatePaymentOrderCommand, Result<CreatePaymentOrderResponseDto>>
{
    public async Task<Result<CreatePaymentOrderResponseDto>> Handle(
        CreatePaymentOrderCommand request, CancellationToken ct)
    {
        var order = await db.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.UserId == request.UserId, ct);

        if (order is null)
            return Result<CreatePaymentOrderResponseDto>.Failure("ORDER_NOT_FOUND", "Order not found.");

        if (order.Status != OrderStatus.Placed)
            return Result<CreatePaymentOrderResponseDto>.Failure(
                "INVALID_ORDER_STATUS", "Payment can only be initiated for placed orders.");

        if (order.PaymentStatus != PaymentStatus.Pending)
            return Result<CreatePaymentOrderResponseDto>.Failure(
                "PAYMENT_ALREADY_PROCESSED", "Payment has already been processed for this order.");

        var payment = await db.Payments
            .FirstOrDefaultAsync(p => p.OrderId == order.Id, ct);

        if (payment is null)
            return Result<CreatePaymentOrderResponseDto>.Failure(
                "PAYMENT_NOT_FOUND", "Payment record not found for this order.");

        // Idempotent: if gateway order already created, return existing
        if (payment.GatewayOrderId is not null)
        {
            return Result<CreatePaymentOrderResponseDto>.Success(
                new CreatePaymentOrderResponseDto(
                    payment.Id, payment.GatewayOrderId, payment.Amount,
                    payment.Currency, payment.PaymentGateway));
        }

        var gatewayResult = await gateway.CreateOrderAsync(
            order.TotalAmount, "INR", order.OrderNumber, ct);

        if (!gatewayResult.Success)
            return Result<CreatePaymentOrderResponseDto>.Failure(
                "GATEWAY_ERROR", gatewayResult.Error ?? "Failed to create payment order.");

        // Update payment record with gateway details
        payment.GatewayOrderId = gatewayResult.GatewayOrderId;
        payment.PaymentGateway = "DevGateway";
        payment.UpdatedAt = DateTimeOffset.UtcNow;

        // Update order payment method
        order.PaymentMethod = (PaymentMethod)request.PaymentMethod;

        await db.SaveChangesAsync(ct);

        return Result<CreatePaymentOrderResponseDto>.Success(
            new CreatePaymentOrderResponseDto(
                payment.Id, gatewayResult.GatewayOrderId!, payment.Amount,
                payment.Currency, payment.PaymentGateway));
    }
}
