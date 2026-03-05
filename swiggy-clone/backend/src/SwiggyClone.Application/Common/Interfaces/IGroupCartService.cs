using SwiggyClone.Application.Features.Cart.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Common.Interfaces;

public interface IGroupCartService
{
    Task<Result<CartDto>> GetParticipantCartAsync(Guid groupOrderId, Guid userId, CancellationToken ct = default);
    Task<Result<CartDto>> AddToCartAsync(Guid groupOrderId, Guid userId, AddToCartItem item, CancellationToken ct = default);
    Task<Result<CartDto>> UpdateQuantityAsync(Guid groupOrderId, Guid userId, string cartItemId, int quantity, CancellationToken ct = default);
    Task<Result<CartDto>> RemoveItemAsync(Guid groupOrderId, Guid userId, string cartItemId, CancellationToken ct = default);
    Task<Result> ClearParticipantCartAsync(Guid groupOrderId, Guid userId, CancellationToken ct = default);
    Task<Result> ClearAllCartsAsync(Guid groupOrderId, CancellationToken ct = default);
    Task<List<(Guid UserId, CartDto Cart)>> GetAllParticipantCartsAsync(Guid groupOrderId, CancellationToken ct = default);
}
