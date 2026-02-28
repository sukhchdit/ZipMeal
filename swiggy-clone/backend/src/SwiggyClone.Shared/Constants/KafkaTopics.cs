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
}
