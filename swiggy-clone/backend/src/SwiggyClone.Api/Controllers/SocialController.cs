using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Social.Commands;
using SwiggyClone.Application.Features.Social.Dtos;
using SwiggyClone.Application.Features.Social.Queries;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/social")]
[Authorize]
public sealed class SocialController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public SocialController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    /// <summary>Follow a user.</summary>
    [HttpPost("follow/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Follow(Guid userId, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new FollowUserCommand(currentUserId, userId), ct);

        if (result.IsSuccess)
            return Ok();

        return result.ErrorCode switch
        {
            "USER_NOT_FOUND" => NotFound(new { result.ErrorCode, result.ErrorMessage }),
            _ => BadRequest(new { result.ErrorCode, result.ErrorMessage }),
        };
    }

    /// <summary>Unfollow a user.</summary>
    [HttpDelete("follow/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Unfollow(Guid userId, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new UnfollowUserCommand(currentUserId, userId), ct);
        return result.IsSuccess
            ? NoContent()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get the activity feed for the current user.</summary>
    [HttpGet("feed")]
    [ProducesResponseType(typeof(ActivityFeedResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeed(
        [FromQuery] DateTimeOffset? cursor,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new GetActivityFeedQuery(userId, cursor, pageSize), ct);
        return Ok(result.Value);
    }

    /// <summary>Get a user's public profile.</summary>
    [HttpGet("profile/{userId:guid}")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile(Guid userId, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;
        var result = await _sender.Send(
            new GetUserProfileQuery(userId, currentUserId), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get a user's followers list.</summary>
    [HttpGet("{userId:guid}/followers")]
    [ProducesResponseType(typeof(List<FollowUserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFollowers(
        Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new GetFollowersQuery(userId, page, pageSize), ct);
        return Ok(result.Value);
    }

    /// <summary>Get users that a user is following.</summary>
    [HttpGet("{userId:guid}/following")]
    [ProducesResponseType(typeof(List<FollowUserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFollowing(
        Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new GetFollowingQuery(userId, page, pageSize), ct);
        return Ok(result.Value);
    }

    /// <summary>Check if the current user follows a target user.</summary>
    [HttpGet("follow/{userId:guid}/status")]
    [ProducesResponseType(typeof(FollowStatusDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckFollowStatus(Guid userId, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new CheckFollowStatusQuery(currentUserId, userId), ct);
        return Ok(result.Value);
    }

    /// <summary>Generate a shareable deep link.</summary>
    [HttpGet("share/{type}/{entityId:guid}")]
    [ProducesResponseType(typeof(ShareLinkDto), StatusCodes.Status200OK)]
    public IActionResult GetShareLink(string type, Guid entityId)
    {
        var shareUrl = $"https://zipmeal.com/{type}/{entityId}";
        var shareText = type.ToLowerInvariant() switch
        {
            "restaurant" => $"Check out this restaurant on ZipMeal! {shareUrl}",
            "review" => $"Read this review on ZipMeal! {shareUrl}",
            "order" => $"Check out my order on ZipMeal! {shareUrl}",
            "profile" => $"Follow me on ZipMeal! {shareUrl}",
            _ => $"Check this out on ZipMeal! {shareUrl}",
        };

        return Ok(new ShareLinkDto(shareUrl, shareText));
    }
}
