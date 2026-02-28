namespace SwiggyClone.Domain.Enums;

public enum PaymentStatus : short
{
    Pending = 0,
    Paid = 1,
    Failed = 2,
    Refunded = 3,
    PartialRefund = 4
}

public enum PaymentMethod : short
{
    Upi = 1,
    Card = 2,
    NetBanking = 3,
    Wallet = 4,
    CashOnDelivery = 5,
    PayAtRestaurant = 6
}
