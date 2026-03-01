using SwiggyClone.Domain.Common;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

public sealed class RestaurantPromotion : BaseEntity
{
    public Guid RestaurantId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public PromotionType PromotionType { get; set; }
    public DiscountType DiscountType { get; set; }
    public int DiscountValue { get; set; }
    public int? MaxDiscount { get; set; }
    public int? MinOrderAmount { get; set; }
    public DateTimeOffset ValidFrom { get; set; }
    public DateTimeOffset ValidUntil { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }

    // Happy Hour specific
    public TimeOnly? RecurringStartTime { get; set; }
    public TimeOnly? RecurringEndTime { get; set; }
    public short[]? RecurringDaysOfWeek { get; set; }

    // Combo Offer specific
    public int? ComboPrice { get; set; }

    // Navigation
    public Restaurant Restaurant { get; set; } = null!;
    public ICollection<PromotionMenuItem> PromotionMenuItems { get; set; } = [];

    public static RestaurantPromotion Create(
        Guid restaurantId,
        string title,
        string? description,
        string? imageUrl,
        PromotionType promotionType,
        DiscountType discountType,
        int discountValue,
        int? maxDiscount,
        int? minOrderAmount,
        DateTimeOffset validFrom,
        DateTimeOffset validUntil,
        int displayOrder,
        TimeOnly? recurringStartTime,
        TimeOnly? recurringEndTime,
        short[]? recurringDaysOfWeek,
        int? comboPrice)
    {
        var now = DateTimeOffset.UtcNow;
        return new RestaurantPromotion
        {
            Id = Guid.CreateVersion7(),
            RestaurantId = restaurantId,
            Title = title,
            Description = description,
            ImageUrl = imageUrl,
            PromotionType = promotionType,
            DiscountType = discountType,
            DiscountValue = discountValue,
            MaxDiscount = maxDiscount,
            MinOrderAmount = minOrderAmount,
            ValidFrom = validFrom,
            ValidUntil = validUntil,
            IsActive = true,
            DisplayOrder = displayOrder,
            RecurringStartTime = recurringStartTime,
            RecurringEndTime = recurringEndTime,
            RecurringDaysOfWeek = recurringDaysOfWeek,
            ComboPrice = comboPrice,
            CreatedAt = now,
            UpdatedAt = now,
        };
    }

    public void Update(
        string title,
        string? description,
        string? imageUrl,
        DiscountType discountType,
        int discountValue,
        int? maxDiscount,
        int? minOrderAmount,
        DateTimeOffset validFrom,
        DateTimeOffset validUntil,
        int displayOrder,
        TimeOnly? recurringStartTime,
        TimeOnly? recurringEndTime,
        short[]? recurringDaysOfWeek,
        int? comboPrice)
    {
        Title = title;
        Description = description;
        ImageUrl = imageUrl;
        DiscountType = discountType;
        DiscountValue = discountValue;
        MaxDiscount = maxDiscount;
        MinOrderAmount = minOrderAmount;
        ValidFrom = validFrom;
        ValidUntil = validUntil;
        DisplayOrder = displayOrder;
        RecurringStartTime = recurringStartTime;
        RecurringEndTime = recurringEndTime;
        RecurringDaysOfWeek = recurringDaysOfWeek;
        ComboPrice = comboPrice;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ToggleActive()
    {
        IsActive = !IsActive;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
