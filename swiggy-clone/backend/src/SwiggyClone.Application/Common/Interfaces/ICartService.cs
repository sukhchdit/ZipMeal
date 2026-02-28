using SwiggyClone.Application.Features.Cart.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Common.Interfaces;

public interface ICartService
{
    Task<Result<CartDto>> GetCartAsync(Guid userId, CancellationToken ct = default);
    Task<Result<CartDto>> AddToCartAsync(Guid userId, AddToCartItem item, CancellationToken ct = default);
    Task<Result<CartDto>> UpdateQuantityAsync(Guid userId, string cartItemId, int quantity, CancellationToken ct = default);
    Task<Result<CartDto>> RemoveItemAsync(Guid userId, string cartItemId, CancellationToken ct = default);
    Task<Result> ClearCartAsync(Guid userId, CancellationToken ct = default);
}

public sealed record AddToCartItem(
    Guid RestaurantId,
    Guid MenuItemId,
    Guid? VariantId,
    int Quantity,
    string? SpecialInstructions,
    List<CartAddonInput> Addons);

public sealed record CartAddonInput(Guid AddonId, int Quantity = 1);
