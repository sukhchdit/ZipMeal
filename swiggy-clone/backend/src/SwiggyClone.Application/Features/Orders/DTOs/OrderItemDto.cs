namespace SwiggyClone.Application.Features.Orders.DTOs;

public sealed record OrderItemDto(
    Guid Id,
    Guid MenuItemId,
    string ItemName,
    Guid? VariantId,
    int Quantity,
    int UnitPrice,
    int TotalPrice,
    string? SpecialInstructions,
    List<OrderItemAddonDto> Addons);
