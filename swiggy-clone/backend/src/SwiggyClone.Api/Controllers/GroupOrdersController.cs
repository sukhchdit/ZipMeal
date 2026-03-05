using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Api.Contracts.GroupOrders;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.GroupOrders.Commands;
using SwiggyClone.Application.Features.GroupOrders.Queries;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/group-orders")]
[Authorize]
public sealed class GroupOrdersController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public GroupOrdersController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    /// <summary>Create a new group order for a restaurant.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateGroupOrder(
        [FromBody] CreateGroupOrderRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new CreateGroupOrderCommand(
            userId, request.RestaurantId, (PaymentSplitType)request.PaymentSplitType,
            request.DeliveryAddressId), ct);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetGroupOrder),
                new { id = result.Value.Id }, result.Value);
        if (result.ErrorCode == "GROUP_ORDER_ALREADY_EXISTS")
            return Conflict(new { result.ErrorCode, result.ErrorMessage });
        return BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Join a group order using an invite code.</summary>
    [HttpPost("join")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> JoinGroupOrder(
        [FromBody] JoinGroupOrderRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new JoinGroupOrderCommand(
            userId, request.InviteCode), ct);

        if (result.IsSuccess) return Ok(result.Value);
        if (result.ErrorCode == "GROUP_ORDER_NOT_FOUND")
            return NotFound(new { result.ErrorCode, result.ErrorMessage });
        if (result.ErrorCode is "ALREADY_PARTICIPANT" or "GROUP_ORDER_ALREADY_EXISTS")
            return Conflict(new { result.ErrorCode, result.ErrorMessage });
        return BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get the current user's active group order (if any).</summary>
    [HttpGet("active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyActiveGroupOrder(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetMyActiveGroupOrderQuery(userId), ct);
        return Ok(result.Value);
    }

    /// <summary>Get detailed information about a group order.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGroupOrder(Guid id, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetGroupOrderQuery(userId, id), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Mark participant as ready.</summary>
    [HttpPut("{id:guid}/ready")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetParticipantReady(Guid id, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new SetParticipantReadyCommand(userId, id), ct);
        return result.IsSuccess
            ? Ok()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Finalize the group order and create a combined delivery order.</summary>
    [HttpPut("{id:guid}/finalize")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> FinalizeGroupOrder(
        Guid id,
        [FromBody] FinalizeGroupOrderRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new FinalizeGroupOrderCommand(
            userId, id, request.DeliveryAddressId, request.PaymentMethod,
            request.CouponCode, request.SpecialInstructions), ct);

        if (result.IsSuccess) return Ok(result.Value);
        if (result.ErrorCode == "NOT_INITIATOR")
            return StatusCode(StatusCodes.Status403Forbidden,
                new { result.ErrorCode, result.ErrorMessage });
        return BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Cancel the group order (initiator only).</summary>
    [HttpPut("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CancelGroupOrder(Guid id, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new CancelGroupOrderCommand(userId, id), ct);

        if (result.IsSuccess) return Ok();
        if (result.ErrorCode == "NOT_INITIATOR")
            return StatusCode(StatusCodes.Status403Forbidden,
                new { result.ErrorCode, result.ErrorMessage });
        return BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Leave the group order (non-initiator only).</summary>
    [HttpPut("{id:guid}/leave")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> LeaveGroupOrder(Guid id, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new LeaveGroupOrderCommand(userId, id), ct);

        if (result.IsSuccess) return Ok();
        if (result.ErrorCode == "INITIATOR_CANNOT_LEAVE")
            return StatusCode(StatusCodes.Status403Forbidden,
                new { result.ErrorCode, result.ErrorMessage });
        return BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Add an item to the participant's group cart.</summary>
    [HttpPost("{id:guid}/cart/items")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddToGroupCart(
        Guid id,
        [FromBody] AddToGroupCartRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var addons = request.Addons?
            .Select(a => new CartAddonInput(a.AddonId, a.Quantity))
            .ToList() ?? [];

        var result = await _sender.Send(new AddToGroupCartCommand(
            userId, id, request.MenuItemId, request.VariantId,
            request.Quantity, request.SpecialInstructions, addons), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Update a cart item's quantity.</summary>
    [HttpPut("{id:guid}/cart/items/{itemId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateGroupCartItem(
        Guid id,
        string itemId,
        [FromBody] UpdateGroupCartItemRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new UpdateGroupCartItemCommand(
            userId, id, itemId, request.Quantity), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Remove a cart item.</summary>
    [HttpDelete("{id:guid}/cart/items/{itemId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveGroupCartItem(
        Guid id,
        string itemId,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new RemoveGroupCartItemCommand(
            userId, id, itemId), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get combined group cart view.</summary>
    [HttpGet("{id:guid}/cart")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetGroupCart(Guid id, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetGroupCartQuery(userId, id), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }
}
