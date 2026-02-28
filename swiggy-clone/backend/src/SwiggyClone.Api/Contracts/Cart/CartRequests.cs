namespace SwiggyClone.Api.Contracts.Cart;

public sealed record AddToCartRequest(
    Guid RestaurantId,
    Guid MenuItemId,
    Guid? VariantId,
    int Quantity,
    string? SpecialInstructions,
    List<CartAddonRequest> Addons);

public sealed record CartAddonRequest(Guid AddonId, int Quantity = 1);

public sealed record UpdateCartItemQuantityRequest(int Quantity);
