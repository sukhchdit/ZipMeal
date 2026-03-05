namespace SwiggyClone.Domain.Enums;

public enum DisputeStatus : short
{
    Opened = 0,
    UnderReview = 1,
    AwaitingCustomerResponse = 2,
    Resolved = 3,
    Closed = 4,
    Escalated = 5,
    Rejected = 6,
}
