using MediatR;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Application.Features.Discovery.Queries;

namespace SwiggyClone.Api.Controllers;

/// <summary>
/// Public customer-facing endpoints for discovering and browsing restaurants.
/// All endpoints are anonymous (no auth required).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/discovery")]
[AllowAnonymous]
public sealed class DiscoveryController : ControllerBase
{
    private readonly ISender _sender;

    public DiscoveryController(ISender sender) => _sender = sender;

    /// <summary>Home feed with banners, cuisine chips, and restaurant sections.</summary>
    [HttpGet("home")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHomeFeed(
        [FromQuery] string? city,
        CancellationToken ct)
    {
        var result = await _sender.Send(new GetHomeFeedQuery(city), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Browse restaurants with filters and cursor pagination.</summary>
    [HttpGet("restaurants")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> BrowseRestaurants(
        [FromQuery] string? city,
        [FromQuery] Guid? cuisineId,
        [FromQuery] bool? isVegOnly,
        [FromQuery] decimal? minRating,
        [FromQuery] int? maxCostForTwo,
        [FromQuery] string? sortBy,
        [FromQuery] string? cursor,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new BrowseRestaurantsQuery(city, cuisineId, isVegOnly, minRating, maxCostForTwo, sortBy, cursor, pageSize),
            ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Search restaurants by name or cuisine keyword.</summary>
    [HttpGet("restaurants/search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchRestaurants(
        [FromQuery] string term,
        [FromQuery] string? city,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(new SearchRestaurantsQuery(term, city, pageSize), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Search menu items (dishes) by name or description via Elasticsearch.</summary>
    [HttpGet("menu-items/search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchMenuItems(
        [FromQuery] string term,
        [FromQuery] string? city,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(new SearchMenuItemsQuery(term, city, pageSize), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get autocomplete suggestions for restaurant names and dish names.</summary>
    [HttpGet("suggestions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSuggestions(
        [FromQuery] string prefix,
        [FromQuery] string? city,
        [FromQuery] int limit = 10,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetSearchSuggestionsQuery(prefix, city, limit), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get full restaurant detail with menu for the customer page.</summary>
    [HttpGet("restaurants/{restaurantId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRestaurantDetail(
        Guid restaurantId,
        CancellationToken ct)
    {
        var result = await _sender.Send(new GetPublicRestaurantDetailQuery(restaurantId), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }
}
