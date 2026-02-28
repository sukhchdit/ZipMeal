namespace SwiggyClone.Api.Contracts.DineIn;

public sealed record StartSessionRequest(
    string QrCodeData,
    int GuestCount = 1);

public sealed record JoinSessionRequest(
    string SessionCode);

public sealed record PlaceDineInOrderRequest(
    List<DineInOrderItemRequest> Items,
    string? SpecialInstructions);

public sealed record DineInOrderItemRequest(
    Guid MenuItemId,
    Guid? VariantId,
    int Quantity,
    string? SpecialInstructions,
    List<DineInOrderAddonRequest>? Addons);

public sealed record DineInOrderAddonRequest(
    Guid AddonId,
    int Quantity = 1);
