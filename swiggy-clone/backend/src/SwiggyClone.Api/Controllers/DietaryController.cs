using MediatR;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Api.Contracts.Dietary;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Dietary.Commands;
using SwiggyClone.Application.Features.Dietary.Queries;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/account/dietary-profile")]
[Authorize]
public sealed class DietaryController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public DietaryController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    /// <summary>Get the current user's dietary profile.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDietaryProfile(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetDietaryProfileQuery(userId), ct);
        return Ok(result.Value);
    }

    /// <summary>Save or update the current user's dietary profile.</summary>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SaveDietaryProfile(
        [FromBody] SaveDietaryProfileRequest request, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new SaveDietaryProfileCommand(
            userId, request.AllergenAlerts, request.DietaryPreferences, request.MaxSpiceLevel), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }
}
