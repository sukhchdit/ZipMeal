using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Api.Contracts.Recommendations;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Recommendations.Commands.TrackInteraction;
using SwiggyClone.Application.Features.Recommendations.Queries.GetPersonalizedRecommendations;
using SwiggyClone.Application.Features.Recommendations.Queries.GetSimilarItems;
using SwiggyClone.Application.Features.Recommendations.Queries.GetSimilarRestaurants;
using SwiggyClone.Application.Features.Recommendations.Queries.GetTrendingItems;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/recommendations")]
public sealed class RecommendationsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public RecommendationsController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    [Authorize]
    [HttpGet("personalized")]
    public async Task<IActionResult> GetPersonalized([FromQuery] string? city)
    {
        var result = await _sender.Send(new GetPersonalizedRecommendationsQuery(
            _currentUser.UserId!.Value, city));

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    [AllowAnonymous]
    [HttpGet("trending")]
    public async Task<IActionResult> GetTrending([FromQuery] string? city, [FromQuery] int count = 20)
    {
        var result = await _sender.Send(new GetTrendingItemsQuery(city, count));

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    [AllowAnonymous]
    [HttpGet("similar-restaurants/{restaurantId:guid}")]
    public async Task<IActionResult> GetSimilarRestaurants(Guid restaurantId, [FromQuery] int count = 10)
    {
        var result = await _sender.Send(new GetSimilarRestaurantsQuery(restaurantId, count));

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    [AllowAnonymous]
    [HttpGet("similar-items/{menuItemId:guid}")]
    public async Task<IActionResult> GetSimilarItems(Guid menuItemId, [FromQuery] int count = 10)
    {
        var result = await _sender.Send(new GetSimilarItemsQuery(menuItemId, count));

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    [Authorize]
    [HttpPost("interactions")]
    public async Task<IActionResult> TrackInteraction([FromBody] TrackInteractionRequest request)
    {
        var result = await _sender.Send(new TrackInteractionCommand(
            _currentUser.UserId!.Value,
            request.EntityType,
            request.EntityId,
            request.InteractionType));

        return result.IsSuccess
            ? Ok()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }
}
