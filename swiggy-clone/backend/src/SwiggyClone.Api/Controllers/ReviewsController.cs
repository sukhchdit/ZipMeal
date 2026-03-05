using MediatR;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Api.Authorization;
using SwiggyClone.Api.Contracts.Reviews;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Reviews.Commands;
using SwiggyClone.Application.Features.Reviews.Commands.DeleteReviewReply;
using SwiggyClone.Application.Features.Reviews.Commands.RemoveReviewVote;
using SwiggyClone.Application.Features.Reviews.Commands.ReportReview;
using SwiggyClone.Application.Features.Reviews.Commands.ResolveReviewReport;
using SwiggyClone.Application.Features.Reviews.Commands.UpdateReviewReply;
using SwiggyClone.Application.Features.Reviews.Commands.UploadReviewPhoto;
using SwiggyClone.Application.Features.Reviews.Commands.VoteReview;
using SwiggyClone.Application.Features.Reviews.Queries;
using SwiggyClone.Application.Features.Reviews.Queries.GetReviewAnalytics;
using SwiggyClone.Application.Features.Reviews.Queries.GetReviewReports;
using SwiggyClone.Domain.Enums;

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
            new GetRestaurantReviewsQuery(restaurantId, page, pageSize, _currentUser.UserId), ct);
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

    /// <summary>Update an existing reply on a review (restaurant owner only).</summary>
    [HttpPatch("{reviewId:guid}/reply")]
    [Authorize(Policy = AuthorizationPolicies.RestaurantOwner)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateReviewReply(
        Guid reviewId,
        [FromBody] ReplyToReviewRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new UpdateReviewReplyCommand(reviewId, userId, request.ReplyText), ct);

        if (result.IsSuccess) return Ok();
        if (result.ErrorCode == "UNAUTHORIZED")
            return StatusCode(StatusCodes.Status403Forbidden, new { result.ErrorCode, result.ErrorMessage });
        return BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Delete a reply from a review (restaurant owner only).</summary>
    [HttpDelete("{reviewId:guid}/reply")]
    [Authorize(Policy = AuthorizationPolicies.RestaurantOwner)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteReviewReply(
        Guid reviewId,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new DeleteReviewReplyCommand(reviewId, userId), ct);

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

    /// <summary>Upload a photo for a review.</summary>
    [HttpPost("upload-photo")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadReviewPhoto(
        [FromForm] IFormFile file,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        using var stream = file.OpenReadStream();
        var result = await _sender.Send(
            new UploadReviewPhotoCommand(userId, stream, file.FileName), ct);

        return result.IsSuccess
            ? Ok(new { Url = result.Value })
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Vote a review as helpful or not helpful.</summary>
    [HttpPost("{reviewId:guid}/vote")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VoteReview(
        Guid reviewId,
        [FromBody] VoteReviewRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new VoteReviewCommand(reviewId, userId, request.IsHelpful), ct);

        return result.IsSuccess
            ? Ok()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Remove your vote from a review.</summary>
    [HttpDelete("{reviewId:guid}/vote")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveReviewVote(
        Guid reviewId,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new RemoveReviewVoteCommand(reviewId, userId), ct);

        return result.IsSuccess
            ? Ok()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Report a review.</summary>
    [HttpPost("{reviewId:guid}/report")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReportReview(
        Guid reviewId,
        [FromBody] ReportReviewRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new ReportReviewCommand(reviewId, userId, request.Reason, request.Description), ct);

        return result.IsSuccess
            ? Ok()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get review analytics for a restaurant (restaurant owner only).</summary>
    [HttpGet("restaurant/{restaurantId:guid}/analytics")]
    [Authorize(Policy = AuthorizationPolicies.RestaurantOwner)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetReviewAnalytics(
        Guid restaurantId,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new GetReviewAnalyticsQuery(restaurantId, userId), ct);

        if (result.IsSuccess) return Ok(result.Value);
        if (result.ErrorCode == "UNAUTHORIZED")
            return StatusCode(StatusCodes.Status403Forbidden, new { result.ErrorCode, result.ErrorMessage });
        return BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get review reports for admin moderation.</summary>
    [HttpGet("admin/reports")]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReviewReports(
        [FromQuery] ReviewReportStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new GetReviewReportsQuery(status, page, pageSize), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Resolve a review report (admin only).</summary>
    [HttpPut("admin/reports/{reportId:guid}/resolve")]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResolveReviewReport(
        Guid reportId,
        [FromBody] ResolveReportRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new ResolveReviewReportCommand(reportId, userId, request.Status, request.AdminNotes), ct);

        return result.IsSuccess
            ? Ok()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }
}
