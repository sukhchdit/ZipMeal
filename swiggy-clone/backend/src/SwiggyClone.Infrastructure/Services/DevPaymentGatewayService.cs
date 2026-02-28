using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;

namespace SwiggyClone.Infrastructure.Services;

/// <summary>
/// Development-only payment gateway that simulates all operations successfully.
/// In production, replace with Razorpay/Stripe implementation.
/// </summary>
internal sealed class DevPaymentGatewayService : IPaymentGatewayService
{
    private readonly ILogger<DevPaymentGatewayService> _logger;

    public DevPaymentGatewayService(ILogger<DevPaymentGatewayService> logger)
    {
        _logger = logger;
    }

    public async Task<GatewayOrderResult> CreateOrderAsync(
        int amountInPaise, string currency, string receiptId, CancellationToken ct = default)
    {
        await Task.Delay(100, ct);

        var gatewayOrderId = $"dev_order_{Guid.NewGuid():N}";

        _logger.LogInformation(
            "[DEV] Payment order created: {GatewayOrderId}, Amount: {Amount} {Currency}, Receipt: {ReceiptId}",
            gatewayOrderId, amountInPaise, currency, receiptId);

        return new GatewayOrderResult(true, gatewayOrderId, null);
    }

    public async Task<GatewayVerifyResult> VerifyPaymentAsync(
        string gatewayOrderId, string gatewayPaymentId, string gatewaySignature,
        CancellationToken ct = default)
    {
        await Task.Delay(100, ct);

        _logger.LogInformation(
            "[DEV] Payment verified: OrderId={GatewayOrderId}, PaymentId={GatewayPaymentId}",
            gatewayOrderId, gatewayPaymentId);

        return new GatewayVerifyResult(true, "upi", null);
    }

    public async Task<GatewayRefundResult> RefundAsync(
        string gatewayPaymentId, int amountInPaise, CancellationToken ct = default)
    {
        await Task.Delay(100, ct);

        var refundId = $"dev_refund_{Guid.NewGuid():N}";

        _logger.LogInformation(
            "[DEV] Refund processed: PaymentId={GatewayPaymentId}, Amount: {Amount}, RefundId: {RefundId}",
            gatewayPaymentId, amountInPaise, refundId);

        return new GatewayRefundResult(true, refundId, null);
    }
}
