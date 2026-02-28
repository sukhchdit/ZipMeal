namespace SwiggyClone.Application.Features.Subscriptions.DTOs;

public sealed record SubscriptionPlanDto(
    Guid Id, string Name, string? Description, int PricePaise, int DurationDays,
    bool FreeDelivery, int ExtraDiscountPercent, bool NoSurgeFee, bool IsActive);
