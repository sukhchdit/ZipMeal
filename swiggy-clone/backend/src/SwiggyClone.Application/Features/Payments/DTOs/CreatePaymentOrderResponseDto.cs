namespace SwiggyClone.Application.Features.Payments.DTOs;

public sealed record CreatePaymentOrderResponseDto(
    Guid PaymentId,
    string GatewayOrderId,
    int Amount,
    string Currency,
    string Gateway);
