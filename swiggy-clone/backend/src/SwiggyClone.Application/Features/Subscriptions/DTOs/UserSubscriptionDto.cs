namespace SwiggyClone.Application.Features.Subscriptions.DTOs;

public sealed record UserSubscriptionDto(
    Guid Id, Guid PlanId, string PlanName, int PaidAmountPaise,
    DateTimeOffset StartDate, DateTimeOffset EndDate, int Status,
    bool FreeDelivery, int ExtraDiscountPercent, bool NoSurgeFee);
