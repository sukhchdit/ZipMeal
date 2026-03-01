namespace SwiggyClone.Api.Contracts.Promotions;

public sealed record CreatePromotionRequest(
    string Title,
    string? Description,
    string? ImageUrl,
    short PromotionType,
    short DiscountType,
    int DiscountValue,
    int? MaxDiscount,
    int? MinOrderAmount,
    DateTimeOffset ValidFrom,
    DateTimeOffset ValidUntil,
    int DisplayOrder,
    string? RecurringStartTime,
    string? RecurringEndTime,
    short[]? RecurringDaysOfWeek,
    int? ComboPrice,
    List<PromotionMenuItemRequest> MenuItems);

public sealed record PromotionMenuItemRequest(Guid MenuItemId, int Quantity = 1);

public sealed record UpdatePromotionRequest(
    string Title,
    string? Description,
    string? ImageUrl,
    short DiscountType,
    int DiscountValue,
    int? MaxDiscount,
    int? MinOrderAmount,
    DateTimeOffset ValidFrom,
    DateTimeOffset ValidUntil,
    int DisplayOrder,
    string? RecurringStartTime,
    string? RecurringEndTime,
    short[]? RecurringDaysOfWeek,
    int? ComboPrice,
    List<PromotionMenuItemRequest> MenuItems);

public sealed record TogglePromotionRequest(bool IsActive);
