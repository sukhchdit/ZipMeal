using MediatR;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Api.Authorization;
using SwiggyClone.Api.Contracts.Deliveries;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Analytics.Queries;
using SwiggyClone.Application.Features.Deliveries.Commands;
using SwiggyClone.Application.Features.Deliveries.Queries;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/deliveries")]
[Authorize]
public sealed class DeliveriesController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public DeliveriesController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    /// <summary>Toggle delivery partner online/offline status.</summary>
    [HttpPut("online-status")]
    [Authorize(Policy = AuthorizationPolicies.DeliveryPartner)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ToggleOnlineStatus(
        [FromBody] ToggleOnlineRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new ToggleOnlineStatusCommand(
            userId, request.IsOnline, request.Latitude, request.Longitude), ct);

        return result.IsSuccess
            ? Ok()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get paginated list of the partner's deliveries.</summary>
    [HttpGet("")]
    [Authorize(Policy = AuthorizationPolicies.DeliveryPartner)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyDeliveries(
        [FromQuery] string? cursor,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new GetMyDeliveriesQuery(userId, cursor, pageSize), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get the partner's current active delivery.</summary>
    [HttpGet("active")]
    [Authorize(Policy = AuthorizationPolicies.DeliveryPartner)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveDelivery(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetActiveDeliveryQuery(userId), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get delivery partner analytics with time series data.</summary>
    [HttpGet("analytics")]
    [Authorize(Policy = AuthorizationPolicies.DeliveryPartner)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPartnerAnalytics(
        [FromQuery] string period = "daily",
        [FromQuery] int days = 30,
        CancellationToken ct = default)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new GetPartnerAnalyticsQuery(userId, period, days), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get the partner's dashboard stats (earnings, delivery counts).</summary>
    [HttpGet("dashboard")]
    [Authorize(Policy = AuthorizationPolicies.DeliveryPartner)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetPartnerDashboardQuery(userId), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Accept a delivery assignment.</summary>
    [HttpPut("{assignmentId:guid}/accept")]
    [Authorize(Policy = AuthorizationPolicies.DeliveryPartner)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AcceptDelivery(
        Guid assignmentId,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new AcceptDeliveryCommand(userId, assignmentId), ct);

        return result.IsSuccess
            ? Ok()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Update delivery status (PickedUp, EnRoute, Delivered).</summary>
    [HttpPut("{assignmentId:guid}/status")]
    [Authorize(Policy = AuthorizationPolicies.DeliveryPartner)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateDeliveryStatus(
        Guid assignmentId,
        [FromBody] UpdateDeliveryStatusRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new UpdateDeliveryStatusCommand(userId, assignmentId, request.NewStatus), ct);

        return result.IsSuccess
            ? Ok()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Update the delivery partner's current location.</summary>
    [HttpPut("location")]
    [Authorize(Policy = AuthorizationPolicies.DeliveryPartner)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateLocation(
        [FromBody] UpdateLocationRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new UpdatePartnerLocationCommand(
            userId, request.Latitude, request.Longitude,
            request.Heading, request.Speed), ct);

        return result.IsSuccess
            ? Ok()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get delivery tracking info for a customer's order.</summary>
    [HttpGet("tracking/{orderId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeliveryTracking(
        Guid orderId,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new GetDeliveryTrackingQuery(userId, orderId), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }
}
