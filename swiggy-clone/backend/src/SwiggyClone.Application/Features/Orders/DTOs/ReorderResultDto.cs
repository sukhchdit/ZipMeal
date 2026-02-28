using SwiggyClone.Application.Features.Cart.DTOs;

namespace SwiggyClone.Application.Features.Orders.DTOs;

public sealed record ReorderResultDto(
    CartDto Cart,
    List<UnavailableReorderItemDto> UnavailableItems);

public sealed record UnavailableReorderItemDto(
    Guid MenuItemId,
    string ItemName,
    string Reason);
