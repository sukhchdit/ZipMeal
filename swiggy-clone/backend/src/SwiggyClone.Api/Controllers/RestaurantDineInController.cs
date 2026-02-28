using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Api.Contracts.DineIn;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.DineIn.Commands;
using SwiggyClone.Application.Features.DineIn.Queries;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[Route("api/v1/restaurants/{restaurantId:guid}")]
[Authorize(Policy = "RestaurantOwner")]
public sealed class RestaurantDineInController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public RestaurantDineInController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    // ──────────────────────────────────────────────
    //  Table Management
    // ──────────────────────────────────────────────

    /// <summary>List all tables for a restaurant.</summary>
    [HttpGet("tables")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetTables(Guid restaurantId, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new GetRestaurantTablesQuery(restaurantId, ownerId), ct);

        if (result.IsSuccess) return Ok(result.Value);
        if (result.ErrorCode is "FORBIDDEN") return Forbid();
        return NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Create a new table.</summary>
    [HttpPost("tables")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateTable(
        Guid restaurantId,
        [FromBody] CreateTableRequest request,
        CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new CreateTableCommand(
            ownerId, restaurantId, request.TableNumber,
            request.Capacity, request.FloorSection), ct);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetTables),
                new { restaurantId }, result.Value);
        if (result.ErrorCode is "TABLE_NUMBER_EXISTS")
            return Conflict(new { result.ErrorCode, result.ErrorMessage });
        return BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Update a table.</summary>
    [HttpPut("tables/{tableId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateTable(
        Guid restaurantId,
        Guid tableId,
        [FromBody] UpdateTableRequest request,
        CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new UpdateTableCommand(
            ownerId, restaurantId, tableId,
            request.TableNumber, request.Capacity, request.FloorSection,
            request.IsActive, request.Status), ct);

        if (result.IsSuccess) return Ok(result.Value);
        if (result.ErrorCode is "TABLE_NOT_FOUND" or "RESTAURANT_NOT_FOUND")
            return NotFound(new { result.ErrorCode, result.ErrorMessage });
        if (result.ErrorCode is "TABLE_NUMBER_EXISTS" or "TABLE_HAS_ACTIVE_SESSION")
            return Conflict(new { result.ErrorCode, result.ErrorMessage });
        return BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Soft-delete a table (set IsActive=false).</summary>
    [HttpDelete("tables/{tableId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteTable(
        Guid restaurantId,
        Guid tableId,
        CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new DeleteTableCommand(ownerId, restaurantId, tableId), ct);

        if (result.IsSuccess) return Ok();
        if (result.ErrorCode is "TABLE_NOT_FOUND" or "RESTAURANT_NOT_FOUND")
            return NotFound(new { result.ErrorCode, result.ErrorMessage });
        if (result.ErrorCode is "TABLE_HAS_ACTIVE_SESSION")
            return Conflict(new { result.ErrorCode, result.ErrorMessage });
        return BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    // ──────────────────────────────────────────────
    //  Active Sessions
    // ──────────────────────────────────────────────

    /// <summary>List active dine-in sessions for a restaurant.</summary>
    [HttpGet("dine-in-sessions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetDineInSessions(
        Guid restaurantId, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new GetRestaurantSessionsQuery(restaurantId, ownerId), ct);

        if (result.IsSuccess) return Ok(result.Value);
        if (result.ErrorCode is "FORBIDDEN") return Forbid();
        return NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    // ──────────────────────────────────────────────
    //  Dine-In Orders
    // ──────────────────────────────────────────────

    /// <summary>List dine-in orders for a restaurant.</summary>
    [HttpGet("dine-in-orders")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetDineInOrders(
        Guid restaurantId,
        [FromQuery] DineInOrderStatus? status,
        CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new GetRestaurantDineInOrdersQuery(restaurantId, ownerId, status), ct);

        if (result.IsSuccess) return Ok(result.Value);
        if (result.ErrorCode is "FORBIDDEN") return Forbid();
        return NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Update a dine-in order's status.</summary>
    [HttpPut("dine-in-orders/{orderId:guid}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDineInOrderStatus(
        Guid restaurantId,
        Guid orderId,
        [FromBody] UpdateDineInOrderStatusRequest request,
        CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new UpdateDineInOrderStatusCommand(
                ownerId, orderId, request.NewStatus, request.Notes), ct);

        if (result.IsSuccess) return Ok();
        if (result.ErrorCode is "ORDER_NOT_FOUND")
            return NotFound(new { result.ErrorCode, result.ErrorMessage });
        return BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }
}
