namespace SwiggyClone.Domain.Entities;

public sealed class PromotionMenuItem
{
    public Guid PromotionId { get; set; }
    public Guid MenuItemId { get; set; }
    public int Quantity { get; set; } = 1;

    public RestaurantPromotion Promotion { get; set; } = null!;
    public MenuItem MenuItem { get; set; } = null!;
}
