using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a payment transaction associated with an <see cref="Order"/>.
/// Captures payment gateway details, status, and refund information.
/// All monetary amounts are stored in paise (smallest currency unit).
/// This entity does not support soft-delete; payment records are immutable audit trails.
/// </summary>
public sealed class Payment
{
    /// <summary>
    /// Unique identifier for this payment record (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="Order"/> this payment is associated with.
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Name of the payment gateway used to process this transaction (max 50 characters),
    /// e.g., "Razorpay", "Stripe", "PayU".
    /// </summary>
    public string PaymentGateway { get; set; } = string.Empty;

    /// <summary>
    /// Unique payment identifier returned by the payment gateway (max 255 characters).
    /// Must be unique across all payments. Null until the gateway processes the payment.
    /// </summary>
    public string? GatewayPaymentId { get; set; }

    /// <summary>
    /// Order identifier created on the payment gateway side (max 255 characters).
    /// Used to correlate the gateway order with the platform order.
    /// </summary>
    public string? GatewayOrderId { get; set; }

    /// <summary>
    /// Cryptographic signature returned by the payment gateway for verification (max 512 characters).
    /// Used to validate the authenticity of the payment callback.
    /// </summary>
    public string? GatewaySignature { get; set; }

    /// <summary>
    /// Total amount charged in paise. Must match the order total at the time of payment.
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    /// ISO 4217 currency code (3 characters). Defaults to "INR".
    /// </summary>
    public string Currency { get; set; } = "INR";

    /// <summary>
    /// Current status of the payment (Pending, Paid, Failed, Refunded, PartialRefund).
    /// Stored as a SMALLINT in the database.
    /// </summary>
    public PaymentStatus Status { get; set; }

    /// <summary>
    /// Payment method used for this transaction (max 50 characters),
    /// e.g., "upi", "card", "netbanking". Null if not yet determined.
    /// </summary>
    public string? Method { get; set; }

    /// <summary>
    /// Additional metadata from the payment gateway stored as a JSON string (JSONB in PostgreSQL).
    /// May include card details (masked), bank name, VPA, etc.
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Amount refunded to the customer in paise. Defaults to 0.
    /// Can be less than <see cref="Amount"/> for partial refunds.
    /// </summary>
    public int? RefundAmount { get; set; } = 0;

    /// <summary>
    /// Reason for the refund (max 500 characters). Null if no refund has been issued.
    /// </summary>
    public string? RefundReason { get; set; }

    /// <summary>
    /// Timestamp when the refund was processed. Null if no refund has been issued.
    /// </summary>
    public DateTimeOffset? RefundedAt { get; set; }

    /// <summary>
    /// Timestamp when this payment record was created (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when this payment record was last updated (UTC).
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The order this payment is associated with.
    /// </summary>
    public Order Order { get; set; } = null!;
}
