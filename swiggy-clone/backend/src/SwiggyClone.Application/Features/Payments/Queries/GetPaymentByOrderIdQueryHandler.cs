using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Payments.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Payments.Queries;

internal sealed class GetPaymentByOrderIdQueryHandler(IAppDbContext db)
    : IRequestHandler<GetPaymentByOrderIdQuery, Result<PaymentDto>>
{
    public async Task<Result<PaymentDto>> Handle(
        GetPaymentByOrderIdQuery request, CancellationToken ct)
    {
        var payment = await db.Payments
            .Include(p => p.Order)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.OrderId == request.OrderId, ct);

        if (payment is null)
            return Result<PaymentDto>.Failure("PAYMENT_NOT_FOUND", "Payment not found.");

        if (payment.Order.UserId != request.UserId)
            return Result<PaymentDto>.Failure("UNAUTHORIZED", "You are not authorized to view this payment.");

        return Result<PaymentDto>.Success(new PaymentDto(
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
            payment.CreatedAt));
    }
}
