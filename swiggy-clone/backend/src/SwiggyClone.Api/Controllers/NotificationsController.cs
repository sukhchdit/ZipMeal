using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Api.Contracts.Notifications;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Notifications.Commands;
using SwiggyClone.Application.Features.Notifications.Queries;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[Route("api/v1/notifications")]
[Authorize]
public sealed class NotificationsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public NotificationsController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    /// <summary>Get paginated list of the current user's notifications.</summary>
    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyNotifications(
        [FromQuery] string? cursor,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new GetMyNotificationsQuery(userId, cursor, pageSize), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get the unread notification count for the current user.</summary>
    [HttpGet("unread-count")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnreadCount(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetUnreadCountQuery(userId), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Mark a single notification as read.</summary>
    [HttpPut("{id:guid}/read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new MarkNotificationAsReadCommand(userId, id), ct);

        return result.IsSuccess
            ? Ok()
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Mark all notifications as read for the current user.</summary>
    [HttpPut("read-all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new MarkAllNotificationsAsReadCommand(userId), ct);

        return result.IsSuccess
            ? Ok()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Register a device for push notifications.</summary>
    [HttpPost("devices")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterDevice(
        [FromBody] RegisterDeviceRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new RegisterDeviceCommand(userId, request.DeviceToken, request.Platform), ct);

        return result.IsSuccess
            ? Ok()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Unregister a device from push notifications.</summary>
    [HttpDelete("devices")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnregisterDevice(
        [FromBody] UnregisterDeviceRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new UnregisterDeviceCommand(userId, request.DeviceToken), ct);

        return result.IsSuccess
            ? Ok()
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }
}
