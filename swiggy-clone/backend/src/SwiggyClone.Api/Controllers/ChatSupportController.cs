using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.ChatSupport.Commands;
using SwiggyClone.Application.Features.ChatSupport.Queries;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
public sealed class ChatSupportController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;
    private readonly IAppDbContext _db;

    public ChatSupportController(ISender sender, ICurrentUserService currentUser, IAppDbContext db)
    {
        _sender = sender;
        _currentUser = currentUser;
        _db = db;
    }

    // ─────────────────── Tickets ─────────────────────────────────────

    /// <summary>Create a new support ticket.</summary>
    [HttpPost("api/v{version:apiVersion}/support/tickets")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTicket(
        [FromBody] CreateTicketRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new CreateTicketCommand(userId, request.Subject, request.Category, request.OrderId, request.InitialMessage), ct);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetTicketMessages), new { id = result.Value.Id }, result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get the current user's support tickets (cursor-paginated).</summary>
    [HttpGet("api/v{version:apiVersion}/support/tickets")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyTickets(
        [FromQuery] string? cursor,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetMyTicketsQuery(userId, cursor, pageSize), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get unread message count across all tickets for the current user.</summary>
    [HttpGet("api/v{version:apiVersion}/support/tickets/unread-count")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnreadCount(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var count = await GetUnreadMessageCount(userId, ct);
        return Ok(new { unreadCount = count });
    }

    // ─────────────────── Messages ────────────────────────────────────

    /// <summary>Get messages for a ticket (cursor-paginated).</summary>
    [HttpGet("api/v{version:apiVersion}/support/tickets/{id:guid}/messages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTicketMessages(
        Guid id,
        [FromQuery] string? cursor,
        [FromQuery] int pageSize = 30,
        CancellationToken ct = default)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetTicketMessagesQuery(userId, id, cursor, pageSize), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Send a message to a ticket.</summary>
    [HttpPost("api/v{version:apiVersion}/support/tickets/{id:guid}/messages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendMessage(
        Guid id,
        [FromBody] SendMessageRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new SendMessageCommand(userId, id, request.Content, request.MessageType), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Mark all unread messages in a ticket as read.</summary>
    [HttpPut("api/v{version:apiVersion}/support/tickets/{id:guid}/messages/read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkMessagesRead(Guid id, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new MarkMessagesReadCommand(userId, id), ct);

        return result.IsSuccess
            ? Ok()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    // ─────────────────── Admin ───────────────────────────────────────

    /// <summary>Close a support ticket (Admin only).</summary>
    [HttpPut("api/v{version:apiVersion}/support/tickets/{id:guid}/close")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CloseTicket(Guid id, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new CloseTicketCommand(userId, id), ct);

        return result.IsSuccess
            ? Ok()
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Assign a support agent to a ticket (Admin only).</summary>
    [HttpPut("api/v{version:apiVersion}/support/tickets/{id:guid}/assign")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignTicket(
        Guid id,
        [FromBody] AssignTicketRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(new AssignTicketCommand(id, request.AgentId), ct);

        return result.IsSuccess
            ? Ok()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get all support tickets (Admin only, with filters).</summary>
    [HttpGet("api/v{version:apiVersion}/admin/support/tickets")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTickets(
        [FromQuery] int? status,
        [FromQuery] int? category,
        [FromQuery] Guid? agentId,
        [FromQuery] string? cursor,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new GetAllTicketsQuery(status, category, agentId, cursor, pageSize), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    // ─────────────────── Canned Responses ────────────────────────────

    /// <summary>Get available canned responses, optionally filtered by category.</summary>
    [HttpGet("api/v{version:apiVersion}/support/canned-responses")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCannedResponses(
        [FromQuery] int? category,
        CancellationToken ct)
    {
        var result = await _sender.Send(new GetCannedResponsesQuery(category), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    // ─────────────────── Private Helpers ─────────────────────────────

    private async Task<int> GetUnreadMessageCount(Guid userId, CancellationToken ct)
    {
        return await _db.SupportTickets.AsNoTracking()
            .Where(t => t.UserId == userId || t.AssignedAgentId == userId)
            .SelectMany(t => t.Messages)
            .CountAsync(m => !m.IsRead && m.SenderId != userId, ct);
    }
}

// ─────────────────── Request DTOs ────────────────────────────────────────

public sealed record CreateTicketRequest(
    string Subject,
    int Category,
    Guid? OrderId,
    string? InitialMessage);

public sealed record SendMessageRequest(
    string Content,
    int MessageType = 0);

public sealed record AssignTicketRequest(
    Guid AgentId);
