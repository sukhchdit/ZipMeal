using SwiggyClone.Domain.Common;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a customer order placed on the platform, supporting delivery, dine-in, and takeaway flows.
/// This is the aggregate root for the Order bounded context. All monetary amounts are stored in paise
/// (smallest currency unit) to avoid floating-point precision issues.
/// Supports soft-delete via <see cref="BaseEntity"/>.
/// </summary>
public sealed class Order : BaseEntity, IAggregateRoot
{
    /// <summary>
    /// Human-readable order number in the format SWG-YYYYMMDD-XXXX.
    /// Must be unique across all orders.
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to the <see cref="User"/> who placed this order.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="Restaurant"/> this order was placed at.
    /// </summary>
    public Guid RestaurantId { get; set; }

    /// <summary>
    /// Type of order: Delivery, DineIn, or Takeaway.
    /// Stored as a SMALLINT in the database.
    /// </summary>
    public OrderType OrderType { get; set; }

    /// <summary>
    /// Current status of the order in its lifecycle (Placed, Confirmed, Preparing, etc.).
    /// Stored as a SMALLINT in the database.
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// Sum of all item prices before tax, fees, and discounts, in paise.
    /// </summary>
    public int Subtotal { get; set; }

    /// <summary>
    /// Total tax amount applied to the order, in paise.
    /// </summary>
    public int TaxAmount { get; set; }

    /// <summary>
    /// Delivery fee charged for the order, in paise. Applicable only for delivery orders.
    /// </summary>
    public int DeliveryFee { get; set; }

    /// <summary>
    /// Packaging charge applied by the restaurant, in paise.
    /// </summary>
    public int PackagingCharge { get; set; }

    /// <summary>
    /// Discount amount applied via coupon or promotion, in paise.
    /// </summary>
    public int DiscountAmount { get; set; }

    /// <summary>
    /// Final amount payable by the customer after all adjustments, in paise.
    /// Calculated as: Subtotal + TaxAmount + DeliveryFee + PackagingCharge - DiscountAmount.
    /// </summary>
    public int TotalAmount { get; set; }

    /// <summary>
    /// Current payment status of the order (Pending, Paid, Failed, Refunded, PartialRefund).
    /// Stored as a SMALLINT in the database.
    /// </summary>
    public PaymentStatus PaymentStatus { get; set; }

    /// <summary>
    /// Payment method selected by the customer (UPI, Card, NetBanking, etc.).
    /// Null until the customer selects a payment method. Stored as a SMALLINT in the database.
    /// </summary>
    public PaymentMethod? PaymentMethod { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="Coupon"/> applied to this order, if any.
    /// </summary>
    public Guid? CouponId { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="UserAddress"/> used as the delivery destination.
    /// Applicable only for delivery orders.
    /// </summary>
    public Guid? DeliveryAddressId { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="User"/> assigned as the delivery partner.
    /// Applicable only for delivery orders after a partner accepts the assignment.
    /// </summary>
    public Guid? DeliveryPartnerId { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="RestaurantTable"/> for dine-in orders.
    /// Applicable only when <see cref="OrderType"/> is <see cref="Enums.OrderType.DineIn"/>.
    /// </summary>
    public Guid? DineInTableId { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="DineInSession"/> this order belongs to.
    /// Applicable only when <see cref="OrderType"/> is <see cref="Enums.OrderType.DineIn"/>.
    /// </summary>
    public Guid? DineInSessionId { get; set; }

    /// <summary>
    /// Estimated time of delivery or readiness, set when the restaurant confirms the order.
    /// </summary>
    public DateTimeOffset? EstimatedDeliveryTime { get; set; }

    /// <summary>
    /// Customer-requested delivery time for scheduled orders. Null for immediate orders.
    /// </summary>
    public DateTimeOffset? ScheduledDeliveryTime { get; set; }

    /// <summary>
    /// Actual time when the order was delivered or served to the customer.
    /// </summary>
    public DateTimeOffset? ActualDeliveryTime { get; set; }

    /// <summary>
    /// Optional special instructions from the customer for the entire order.
    /// </summary>
    public string? SpecialInstructions { get; set; }

    /// <summary>
    /// Reason provided when the order is cancelled (max 500 characters).
    /// </summary>
    public string? CancellationReason { get; set; }

    /// <summary>
    /// Indicates who cancelled the order. Stored as a SMALLINT.
    /// Null if the order has not been cancelled.
    /// </summary>
    public short? CancelledBy { get; set; }

    /// <summary>
    /// Tip amount given by the customer to the delivery partner, in paise.
    /// Zero indicates no tip has been given. Only applicable for delivery orders.
    /// </summary>
    public int TipAmount { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The customer who placed this order.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// The restaurant that fulfills this order.
    /// </summary>
    public Restaurant Restaurant { get; set; } = null!;

    /// <summary>
    /// Line items included in this order.
    /// </summary>
    public ICollection<OrderItem> Items { get; set; } = [];

    /// <summary>
    /// Chronological history of status transitions for this order.
    /// </summary>
    public ICollection<OrderStatusHistory> StatusHistory { get; set; } = [];

    /// <summary>
    /// Delivery address associated with this order. Null for dine-in and takeaway orders.
    /// </summary>
    public UserAddress? DeliveryAddress { get; set; }

    /// <summary>
    /// Delivery partner assigned to this order. Null until a partner is assigned.
    /// </summary>
    public User? DeliveryPartner { get; set; }

    /// <summary>
    /// Dine-in table associated with this order. Null for delivery and takeaway orders.
    /// </summary>
    public RestaurantTable? DineInTable { get; set; }

    /// <summary>
    /// Dine-in session this order belongs to. Null for delivery and takeaway orders.
    /// </summary>
    public DineInSession? DineInSession { get; set; }

    /// <summary>
    /// Payment record associated with this order.
    /// </summary>
    public Payment? Payment { get; set; }
}
