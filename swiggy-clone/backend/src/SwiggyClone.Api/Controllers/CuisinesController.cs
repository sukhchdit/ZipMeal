using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Application.Features.Restaurants.Queries;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[Route("api/v1/cuisines")]
public sealed class CuisinesController : ControllerBase
{
    private readonly ISender _sender;

    public CuisinesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Get all available cuisine types.</summary>
    [HttpGet("")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCuisineTypes(CancellationToken ct)
    {
        var result = await _sender.Send(new GetCuisineTypesQuery(), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }
}
