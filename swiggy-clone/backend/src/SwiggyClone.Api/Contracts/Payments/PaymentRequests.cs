namespace SwiggyClone.Api.Contracts.Payments;

public sealed record CreatePaymentOrderRequest(Guid OrderId, int PaymentMethod);

public sealed record VerifyPaymentRequest(
    string GatewayOrderId,
    string GatewayPaymentId,
    string GatewaySignature);

public sealed record InitiateRefundRequest(string? Reason);

public sealed record PayDineInSessionRequest(Guid SessionId, int PaymentMethod);
