using MediatR;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Api.Contracts.Cart;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Cart.Commands;
using SwiggyClone.Application.Features.Cart.Queries;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/cart")]
[Authorize]
public sealed class CartController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public CartController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    /// <summary>Get the current user's cart.</summary>
    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCart(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetCartQuery(userId), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Add an item to the cart.</summary>
    [HttpPost("items")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddToCart(
        [FromBody] AddToCartRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var addons = request.Addons
            .Select(a => new CartAddonInput(a.AddonId, a.Quantity))
            .ToList();

        var result = await _sender.Send(new AddToCartCommand(
            userId,
            request.RestaurantId,
            request.MenuItemId,
            request.VariantId,
            request.Quantity,
            request.SpecialInstructions,
            addons), ct);

        if (result.IsSuccess) return Ok(result.Value);
        if (result.ErrorCode == "DIFFERENT_RESTAURANT")
            return Conflict(new { result.ErrorCode, result.ErrorMessage });
        return BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Update the quantity of a cart item.</summary>
    [HttpPut("items/{cartItemId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateQuantity(
        string cartItemId,
        [FromBody] UpdateCartItemQuantityRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new UpdateCartItemQuantityCommand(userId, cartItemId, request.Quantity), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Remove an item from the cart.</summary>
    [HttpDelete("items/{cartItemId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveItem(
        string cartItemId,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new RemoveCartItemCommand(userId, cartItemId), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Clear the entire cart.</summary>
    [HttpDelete("")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ClearCart(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new ClearCartCommand(userId), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }
}
