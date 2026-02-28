using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Application.Features.Payments.DTOs;

public sealed record PaymentDto(
    Guid Id,
    Guid OrderId,
    string PaymentGateway,
    string? GatewayPaymentId,
    string? GatewayOrderId,
    int Amount,
    string Currency,
    PaymentStatus Status,
    string? Method,
    int? RefundAmount,
    string? RefundReason,
    DateTimeOffset? RefundedAt,
    DateTimeOffset CreatedAt);
