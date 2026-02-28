namespace SwiggyClone.Api.Contracts.Subscriptions;

public sealed record CreatePlanRequest(
    string Name, string? Description, int PricePaise, int DurationDays,
    bool FreeDelivery, int ExtraDiscountPercent, bool NoSurgeFee);

public sealed record UpdatePlanRequest(
    string Name, string? Description, int PricePaise, int DurationDays,
    bool FreeDelivery, int ExtraDiscountPercent, bool NoSurgeFee);

public sealed record TogglePlanRequest(bool IsActive);

public sealed record SubscribeToPlanRequest(Guid PlanId);
