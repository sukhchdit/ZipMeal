using SwiggyClone.Application.Features.Cart.DTOs;

namespace SwiggyClone.Application.Features.GroupOrders.DTOs;

public sealed record GroupCartDto(
    Guid GroupOrderId,
    List<GroupParticipantCartDto> ParticipantCarts,
    int GrandTotal);

public sealed record GroupParticipantCartDto(
    Guid UserId,
    string UserName,
    List<CartItemDto> Items,
    int Subtotal);
