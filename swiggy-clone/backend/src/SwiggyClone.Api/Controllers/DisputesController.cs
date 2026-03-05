using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Api.Contracts.Disputes;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Disputes.Commands.AddDisputeMessage;
using SwiggyClone.Application.Features.Disputes.Commands.AssignDispute;
using SwiggyClone.Application.Features.Disputes.Commands.CreateDispute;
using SwiggyClone.Application.Features.Disputes.Commands.RejectDispute;
using SwiggyClone.Application.Features.Disputes.Commands.ResolveDispute;
using SwiggyClone.Application.Features.Disputes.Queries.GetAllDisputes;
using SwiggyClone.Application.Features.Disputes.Queries.GetDisputeDetail;
using SwiggyClone.Application.Features.Disputes.Queries.GetDisputeMessages;
using SwiggyClone.Application.Features.Disputes.Queries.GetMyDisputes;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
public sealed class DisputesController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public DisputesController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    // ─────────────────── Customer Endpoints ──────────────────────────

    /// <summary>Create a new dispute for an order.</summary>
    [HttpPost("api/v{version:apiVersion}/disputes")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateDispute(
        [FromBody] CreateDisputeRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new CreateDisputeCommand(userId, request.OrderId, request.IssueType, request.Description), ct);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetDisputeDetail), new { id = result.Value.Id }, result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get the current user's disputes (cursor-paginated).</summary>
    [HttpGet("api/v{version:apiVersion}/disputes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyDisputes(
        [FromQuery] string? cursor,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetMyDisputesQuery(userId, cursor, pageSize), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get dispute detail.</summary>
    [HttpGet("api/v{version:apiVersion}/disputes/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDisputeDetail(Guid id, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetDisputeDetailQuery(userId, id), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get messages for a dispute (cursor-paginated).</summary>
    [HttpGet("api/v{version:apiVersion}/disputes/{id:guid}/messages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDisputeMessages(
        Guid id,
        [FromQuery] string? cursor,
        [FromQuery] int pageSize = 30,
        CancellationToken ct = default)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetDisputeMessagesQuery(userId, id, cursor, pageSize), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Add a message to a dispute.</summary>
    [HttpPost("api/v{version:apiVersion}/disputes/{id:guid}/messages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddMessage(
        Guid id,
        [FromBody] AddDisputeMessageRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new AddDisputeMessageCommand(userId, id, request.Content), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    // ─────────────────── Admin Endpoints ─────────────────────────────

    /// <summary>Assign an agent to a dispute (Admin only).</summary>
    [HttpPut("api/v{version:apiVersion}/admin/disputes/{id:guid}/assign")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignDispute(
        Guid id,
        [FromBody] AssignDisputeRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(new AssignDisputeCommand(id, request.AgentId), ct);

        return result.IsSuccess
            ? Ok()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Resolve a dispute (Admin only).</summary>
    [HttpPut("api/v{version:apiVersion}/admin/disputes/{id:guid}/resolve")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResolveDispute(
        Guid id,
        [FromBody] ResolveDisputeRequest request,
        CancellationToken ct)
    {
        var agentId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new ResolveDisputeCommand(agentId, id, request.ResolutionType, request.ResolutionAmountPaise, request.ResolutionNotes), ct);

        return result.IsSuccess
            ? Ok()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Reject a dispute (Admin only).</summary>
    [HttpPut("api/v{version:apiVersion}/admin/disputes/{id:guid}/reject")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RejectDispute(
        Guid id,
        [FromBody] RejectDisputeRequest request,
        CancellationToken ct)
    {
        var agentId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new RejectDisputeCommand(agentId, id, request.Reason), ct);

        return result.IsSuccess
            ? Ok()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get all disputes (Admin only, with filters).</summary>
    [HttpGet("api/v{version:apiVersion}/admin/disputes")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllDisputes(
        [FromQuery] int? status,
        [FromQuery] int? issueType,
        [FromQuery] Guid? agentId,
        [FromQuery] string? cursor,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new GetAllDisputesQuery(status, issueType, agentId, cursor, pageSize), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }
}
