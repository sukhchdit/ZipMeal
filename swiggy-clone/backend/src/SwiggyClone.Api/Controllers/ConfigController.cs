using MediatR;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Application.Features.PlatformConfig.Queries;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/config")]
[Authorize]
public sealed class ConfigController : ControllerBase
{
    private readonly ISender _sender;

    public ConfigController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Get the current platform fee configuration for checkout display.</summary>
    [HttpGet("fees")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFees(CancellationToken ct)
    {
        var result = await _sender.Send(new GetPlatformConfigQuery(), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }
}
