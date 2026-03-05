namespace SwiggyClone.Api.Contracts.GroupOrders;

public sealed record CreateGroupOrderRequest(
    Guid RestaurantId,
    int PaymentSplitType = 0,
    Guid? DeliveryAddressId = null);

public sealed record JoinGroupOrderRequest(
    string InviteCode);

public sealed record FinalizeGroupOrderRequest(
    Guid DeliveryAddressId,
    int PaymentMethod,
    string? CouponCode = null,
    string? SpecialInstructions = null);

public sealed record AddToGroupCartRequest(
    Guid MenuItemId,
    Guid? VariantId,
    int Quantity = 1,
    string? SpecialInstructions = null,
    List<GroupCartAddonRequest>? Addons = null);

public sealed record GroupCartAddonRequest(
    Guid AddonId,
    int Quantity = 1);

public sealed record UpdateGroupCartItemRequest(
    int Quantity);
