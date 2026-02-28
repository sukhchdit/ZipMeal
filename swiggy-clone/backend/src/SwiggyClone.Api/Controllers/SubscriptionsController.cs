using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Api.Contracts.Subscriptions;
using SwiggyClone.Application.Features.Subscriptions.Commands.CancelSubscription;
using SwiggyClone.Application.Features.Subscriptions.Commands.SubscribeToPlan;
using SwiggyClone.Application.Features.Subscriptions.Queries.GetAvailablePlans;
using SwiggyClone.Application.Features.Subscriptions.Queries.GetMySubscription;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/subscriptions")]
[Authorize]
public sealed class SubscriptionsController : ControllerBase
{
    private readonly ISender _sender;

    public SubscriptionsController(ISender sender)
    {
        _sender = sender;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Get all active subscription plans.</summary>
    [HttpGet("plans")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailablePlans(CancellationToken ct)
    {
        var result = await _sender.Send(new GetAvailablePlansQuery(), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get the current user's active subscription.</summary>
    [HttpGet("my")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMySubscription(CancellationToken ct)
    {
        var result = await _sender.Send(new GetMySubscriptionQuery(GetUserId()), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Subscribe to a plan.</summary>
    [HttpPost("subscribe")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Subscribe(
        [FromBody] SubscribeToPlanRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new SubscribeToPlanCommand(GetUserId(), request.PlanId), ct);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Cancel the current active subscription.</summary>
    [HttpPost("cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cancel(CancellationToken ct)
    {
        var result = await _sender.Send(new CancelSubscriptionCommand(GetUserId()), ct);
        return result.IsSuccess
            ? Ok()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }
}
