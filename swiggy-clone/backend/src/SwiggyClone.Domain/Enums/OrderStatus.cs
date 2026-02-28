namespace SwiggyClone.Domain.Enums;

public enum OrderStatus : short
{
    Placed = 0,
    Confirmed = 1,
    Preparing = 2,
    ReadyForPickup = 3,
    OutForDelivery = 4,
    Delivered = 5,
    Cancelled = 6,
    Scheduled = 7
}

public enum DineInOrderStatus : short
{
    Placed = 0,
    Confirmed = 1,
    Preparing = 2,
    Ready = 3,
    Served = 4,
    Completed = 5,
    Cancelled = 6
}
