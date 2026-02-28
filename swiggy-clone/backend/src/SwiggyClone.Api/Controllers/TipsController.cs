using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Api.Contracts.Tips;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Tips.Commands;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/orders")]
public sealed class TipsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public TipsController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    /// <summary>Submit a tip for a delivered order's delivery partner.</summary>
    [HttpPost("{orderId:guid}/tip")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitTip(Guid orderId, [FromBody] SubmitTipRequest request, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new SubmitTipCommand(userId, orderId, request.AmountPaise), ct);

        return result.IsSuccess ? Ok() : BadRequest(result);
    }
}
