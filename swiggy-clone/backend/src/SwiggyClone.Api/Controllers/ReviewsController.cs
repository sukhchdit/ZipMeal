using MediatR;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Api.Authorization;
using SwiggyClone.Api.Contracts.Reviews;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Reviews.Commands;
using SwiggyClone.Application.Features.Reviews.Queries;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reviews")]
public sealed class ReviewsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public ReviewsController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    /// <summary>Submit a review for a delivered order.</summary>
    [HttpPost("")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitReview(
        [FromBody] SubmitReviewRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new SubmitReviewCommand(
            userId,
            request.OrderId,
            request.Rating,
            request.ReviewText,
            request.DeliveryRating,
            request.IsAnonymous,
            request.PhotoUrls ?? []), ct);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetRestaurantReviews),
                new { restaurantId = result.Value.RestaurantId }, result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get paginated reviews for a restaurant (public).</summary>
    [HttpGet("restaurant/{restaurantId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRestaurantReviews(
        Guid restaurantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new GetRestaurantReviewsQuery(restaurantId, page, pageSize), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get the current user's reviews.</summary>
    [HttpGet("my")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyReviews(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new GetMyReviewsQuery(userId, page, pageSize), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Reply to a review (restaurant owner only).</summary>
    [HttpPut("{reviewId:guid}/reply")]
    [Authorize(Policy = AuthorizationPolicies.RestaurantOwner)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReplyToReview(
        Guid reviewId,
        [FromBody] ReplyToReviewRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new ReplyToReviewCommand(reviewId, userId, request.ReplyText), ct);

        if (result.IsSuccess) return Ok();
        if (result.ErrorCode == "UNAUTHORIZED")
            return StatusCode(StatusCodes.Status403Forbidden, new { result.ErrorCode, result.ErrorMessage });
        return BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Toggle review visibility (admin moderation).</summary>
    [HttpPut("{reviewId:guid}/visibility")]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ToggleReviewVisibility(
        Guid reviewId,
        [FromBody] ToggleReviewVisibilityRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new ToggleReviewVisibilityCommand(reviewId, request.IsVisible), ct);
        return result.IsSuccess
            ? Ok()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }
}
