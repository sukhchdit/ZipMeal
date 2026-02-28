using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Api.Contracts.DineIn;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.DineIn.Commands;
using SwiggyClone.Application.Features.DineIn.Queries;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[Route("api/v1/dine-in")]
[Authorize]
public sealed class DineInController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public DineInController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    /// <summary>Start a new dine-in session by scanning a table QR code.</summary>
    [HttpPost("sessions")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> StartSession(
        [FromBody] StartSessionRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new StartSessionCommand(
            userId, request.QrCodeData, request.GuestCount), ct);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetSessionDetail),
                new { sessionId = result.Value.Id }, result.Value);
        if (result.ErrorCode is "ACTIVE_SESSION_EXISTS" or "TABLE_OCCUPIED")
            return Conflict(new { result.ErrorCode, result.ErrorMessage });
        return BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Join an existing dine-in session using a session code.</summary>
    [HttpPost("sessions/join")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> JoinSession(
        [FromBody] JoinSessionRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new JoinSessionCommand(
            userId, request.SessionCode), ct);

        if (result.IsSuccess) return Ok(result.Value);
        if (result.ErrorCode == "SESSION_NOT_FOUND")
            return NotFound(new { result.ErrorCode, result.ErrorMessage });
        if (result.ErrorCode is "ALREADY_MEMBER" or "ACTIVE_SESSION_EXISTS")
            return Conflict(new { result.ErrorCode, result.ErrorMessage });
        return BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get the current user's active dine-in session (if any).</summary>
    [HttpGet("sessions/active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyActiveSession(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new GetMyActiveSessionQuery(userId), ct);
        return Ok(result.Value);
    }

    /// <summary>Get detailed information about a dine-in session.</summary>
    [HttpGet("sessions/{sessionId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSessionDetail(
        Guid sessionId, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new GetSessionDetailQuery(userId, sessionId), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Browse the dine-in menu for the session's restaurant.</summary>
    [HttpGet("sessions/{sessionId:guid}/menu")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDineInMenu(
        Guid sessionId, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new GetDineInMenuQuery(userId, sessionId), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Place a dine-in order within the session.</summary>
    [HttpPost("sessions/{sessionId:guid}/orders")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PlaceDineInOrder(
        Guid sessionId,
        [FromBody] PlaceDineInOrderRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var items = request.Items.Select(i => new DineInOrderItemInput(
            i.MenuItemId,
            i.VariantId,
            i.Quantity,
            i.SpecialInstructions,
            i.Addons?.Select(a => new DineInOrderAddonInput(a.AddonId, a.Quantity)).ToList()
                ?? []
        )).ToList();

        var result = await _sender.Send(new PlaceDineInOrderCommand(
            userId, sessionId, items, request.SpecialInstructions), ct);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get all orders for a dine-in session.</summary>
    [HttpGet("sessions/{sessionId:guid}/orders")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSessionOrders(
        Guid sessionId, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new GetSessionOrdersQuery(userId, sessionId), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Request the bill (host only).</summary>
    [HttpPut("sessions/{sessionId:guid}/request-bill")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RequestBill(
        Guid sessionId, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new RequestBillCommand(userId, sessionId), ct);

        if (result.IsSuccess) return Ok();
        if (result.ErrorCode == "NOT_HOST")
            return StatusCode(StatusCodes.Status403Forbidden,
                new { result.ErrorCode, result.ErrorMessage });
        return BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>End the session after payment (host only).</summary>
    [HttpPut("sessions/{sessionId:guid}/end")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> EndSession(
        Guid sessionId, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new EndSessionCommand(userId, sessionId), ct);

        if (result.IsSuccess) return Ok();
        if (result.ErrorCode == "NOT_HOST")
            return StatusCode(StatusCodes.Status403Forbidden,
                new { result.ErrorCode, result.ErrorMessage });
        return BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Leave a session (guests only; host must end the session).</summary>
    [HttpPut("sessions/{sessionId:guid}/leave")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> LeaveSession(
        Guid sessionId, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new LeaveSessionCommand(userId, sessionId), ct);

        if (result.IsSuccess) return Ok();
        if (result.ErrorCode == "HOST_CANNOT_LEAVE")
            return StatusCode(StatusCodes.Status403Forbidden,
                new { result.ErrorCode, result.ErrorMessage });
        return BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }
}
