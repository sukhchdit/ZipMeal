using MediatR;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

public sealed record CreateMenuItemCommand(
    Guid RestaurantId,
    Guid OwnerId,
    Guid CategoryId,
    string Name,
    string? Description,
    int Price,
    int? DiscountedPrice,
    string? ImageUrl,
    bool IsVeg,
    bool IsAvailable,
    bool IsBestseller,
    int PreparationTimeMin,
    int SortOrder,
    short SpiceLevel,
    short[]? Allergens,
    short[]? DietaryTags,
    int? CalorieCount,
    List<CreateVariantDto>? Variants,
    List<CreateAddonDto>? Addons) : IRequest<Result<MenuItemDto>>;
