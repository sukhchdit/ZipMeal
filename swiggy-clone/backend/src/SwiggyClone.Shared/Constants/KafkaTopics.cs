namespace SwiggyClone.Shared.Constants;

public static class KafkaTopics
{
    public const string OrderCreated = "order.created";
    public const string OrderConfirmed = "order.confirmed";
    public const string OrderPreparing = "order.preparing";
    public const string OrderReady = "order.ready";
    public const string OrderDelivered = "order.delivered";
    public const string OrderCancelled = "order.cancelled";
    public const string PaymentCompleted = "payment.completed";
    public const string PaymentFailed = "payment.failed";
    public const string DeliveryAssigned = "delivery.assigned";
    public const string DeliveryLocationUpdated = "delivery.location.updated";
    public const string DineInSessionStarted = "dinein.session.started";
    public const string DineInOrderPlaced = "dinein.order.placed";
    public const string DineInBillRequested = "dinein.bill.requested";
    public const string NotificationSend = "notification.send";
    public const string RestaurantMenuUpdated = "restaurant.menu.updated";
    public const string ReviewSubmitted = "review.submitted";
    public const string GroupOrderCreated = "group-order.created";
    public const string GroupOrderFinalized = "group-order.finalized";
    public const string GroupOrderCancelled = "group-order.cancelled";
    public const string LoyaltyPointsEarned = "loyalty.points.earned";
    public const string LoyaltyRewardRedeemed = "loyalty.reward.redeemed";
    public const string DisputeCreated = "dispute.created";
    public const string DisputeResolved = "dispute.resolved";
    public const string DisputeEscalated = "dispute.escalated";
    public const string UserInteractionTracked = "user.interaction.tracked";
}
