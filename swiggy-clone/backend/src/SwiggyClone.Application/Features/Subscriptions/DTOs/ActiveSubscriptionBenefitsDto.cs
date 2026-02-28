namespace SwiggyClone.Application.Features.Subscriptions.DTOs;

public sealed record ActiveSubscriptionBenefitsDto(
    bool HasActiveSubscription, bool FreeDelivery, int ExtraDiscountPercent, bool NoSurgeFee);
