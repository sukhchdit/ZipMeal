namespace SwiggyClone.Api.Contracts.Restaurants;

public sealed record CreateMenuItemRequest(
    Guid CategoryId, string Name, string? Description,
    int Price, int? DiscountedPrice, string? ImageUrl,
    bool IsVeg, bool IsAvailable, bool IsBestseller,
    int PreparationTimeMin, int SortOrder,
    List<CreateVariantRequest>? Variants, List<CreateAddonRequest>? Addons);

public sealed record UpdateMenuItemRequest(
    Guid CategoryId, string Name, string? Description,
    int Price, int? DiscountedPrice, string? ImageUrl,
    bool IsVeg, bool IsAvailable, bool IsBestseller,
    int PreparationTimeMin, int SortOrder);

public sealed record CreateVariantRequest(string Name, int PriceAdjustment, bool IsDefault, bool IsAvailable, int SortOrder);
public sealed record UpdateVariantRequest(string Name, int PriceAdjustment, bool IsDefault, bool IsAvailable, int SortOrder);

public sealed record CreateAddonRequest(string Name, int Price, bool IsVeg, bool IsAvailable, int MaxQuantity, int SortOrder);
public sealed record UpdateAddonRequest(string Name, int Price, bool IsVeg, bool IsAvailable, int MaxQuantity, int SortOrder);
