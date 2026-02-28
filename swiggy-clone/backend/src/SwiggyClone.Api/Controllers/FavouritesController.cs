using MediatR;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Favourites.Commands;
using SwiggyClone.Application.Features.Favourites.Queries;

namespace SwiggyClone.Api.Controllers;

/// <summary>
/// Authenticated endpoints for managing user restaurant favourites.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/favourites")]
[Authorize]
public sealed class FavouritesController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public FavouritesController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    /// <summary>Get all favourited restaurants for the current user.</summary>
    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFavourites(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetFavouritesQuery(userId), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Check if a restaurant is favourited by the current user.</summary>
    [HttpGet("{restaurantId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckFavourite(
        Guid restaurantId,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new CheckFavouriteQuery(userId, restaurantId), ct);
        return result.IsSuccess
            ? Ok(new { isFavourited = result.Value })
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Add a restaurant to favourites.</summary>
    [HttpPost("{restaurantId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddFavourite(
        Guid restaurantId,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new AddFavouriteCommand(userId, restaurantId), ct);
        return result.IsSuccess
            ? Ok()
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Remove a restaurant from favourites.</summary>
    [HttpDelete("{restaurantId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveFavourite(
        Guid restaurantId,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new RemoveFavouriteCommand(userId, restaurantId), ct);
        return result.IsSuccess
            ? NoContent()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }
}
