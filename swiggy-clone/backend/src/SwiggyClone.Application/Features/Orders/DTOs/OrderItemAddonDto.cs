namespace SwiggyClone.Application.Features.Orders.DTOs;

public sealed record OrderItemAddonDto(
    Guid AddonId,
    string AddonName,
    int Quantity,
    int Price);
