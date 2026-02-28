namespace SwiggyClone.Application.Common.Interfaces;

/// <summary>
/// Abstracts payment gateway operations (create order, verify payment, refund).
/// In development, all operations succeed with simulated delays.
/// Swap to Razorpay/Stripe in production.
/// </summary>
public interface IPaymentGatewayService
{
    /// <summary>
    /// Creates a payment order on the gateway. Returns a gateway order ID
    /// that the client uses to initiate payment.
    /// </summary>
    Task<GatewayOrderResult> CreateOrderAsync(
        int amountInPaise, string currency, string receiptId, CancellationToken ct = default);

    /// <summary>
    /// Verifies a payment using the gateway's signature verification.
    /// </summary>
    Task<GatewayVerifyResult> VerifyPaymentAsync(
        string gatewayOrderId, string gatewayPaymentId, string gatewaySignature,
        CancellationToken ct = default);

    /// <summary>
    /// Initiates a full refund for a given gateway payment ID.
    /// </summary>
    Task<GatewayRefundResult> RefundAsync(
        string gatewayPaymentId, int amountInPaise, CancellationToken ct = default);
}

public sealed record GatewayOrderResult(bool Success, string? GatewayOrderId, string? Error);
public sealed record GatewayVerifyResult(bool IsValid, string? PaymentMethod, string? Error);
public sealed record GatewayRefundResult(bool Success, string? RefundId, string? Error);
