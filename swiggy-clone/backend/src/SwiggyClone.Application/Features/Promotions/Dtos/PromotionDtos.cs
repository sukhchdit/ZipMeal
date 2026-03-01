namespace SwiggyClone.Application.Features.Promotions.Dtos;

public sealed record PromotionDto(
    Guid Id,
    Guid RestaurantId,
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
    bool IsActive,
    int DisplayOrder,
    string? RecurringStartTime,
    string? RecurringEndTime,
    short[]? RecurringDaysOfWeek,
    int? ComboPrice,
    List<PromotionMenuItemDto> MenuItems,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record PromotionListDto(
    Guid Id,
    Guid RestaurantId,
    string Title,
    short PromotionType,
    short DiscountType,
    int DiscountValue,
    int? MaxDiscount,
    DateTimeOffset ValidFrom,
    DateTimeOffset ValidUntil,
    bool IsActive,
    int DisplayOrder,
    int? ComboPrice,
    int MenuItemCount,
    DateTimeOffset CreatedAt);

public sealed record PromotionMenuItemDto(
    Guid MenuItemId,
    string MenuItemName,
    int Price,
    int? DiscountedPrice,
    int Quantity);
