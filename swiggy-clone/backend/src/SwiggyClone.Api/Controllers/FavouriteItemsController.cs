using MediatR;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.FavouriteItems.Commands;
using SwiggyClone.Application.Features.FavouriteItems.Queries;

namespace SwiggyClone.Api.Controllers;

/// <summary>
/// Authenticated endpoints for managing user favourite menu items (dishes).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/favourites/items")]
[Authorize]
public sealed class FavouriteItemsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public FavouriteItemsController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    /// <summary>Get all favourite menu items for the current user.</summary>
    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFavouriteItems(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetFavouriteItemsQuery(userId), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Check if a menu item is favourited by the current user.</summary>
    [HttpGet("{menuItemId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckFavouriteItem(
        Guid menuItemId,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new CheckFavouriteItemQuery(userId, menuItemId), ct);
        return result.IsSuccess
            ? Ok(new { isFavourited = result.Value })
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Add a menu item to favourites.</summary>
    [HttpPost("{menuItemId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddFavouriteItem(
        Guid menuItemId,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new AddFavouriteItemCommand(userId, menuItemId), ct);
        return result.IsSuccess
            ? Ok()
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Remove a menu item from favourites.</summary>
    [HttpDelete("{menuItemId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveFavouriteItem(
        Guid menuItemId,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new RemoveFavouriteItemCommand(userId, menuItemId), ct);
        return result.IsSuccess
            ? NoContent()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }
}
