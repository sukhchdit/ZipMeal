namespace SwiggyClone.Application.Features.Cart.DTOs;

public sealed record CartDto(
    Guid RestaurantId,
    string RestaurantName,
    List<CartItemDto> Items,
    int Subtotal);

public sealed record CartItemDto(
    string CartItemId,
    Guid MenuItemId,
    Guid? VariantId,
    string ItemName,
    string? VariantName,
    int Quantity,
    int UnitPrice,
    int TotalPrice,
    List<CartItemAddonDto> Addons,
    string? SpecialInstructions);

public sealed record CartItemAddonDto(
    Guid AddonId,
    string AddonName,
    int Price,
    int Quantity);
