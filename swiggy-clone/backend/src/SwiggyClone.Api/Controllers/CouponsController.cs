using MediatR;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Coupons.Queries;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/coupons")]
[Authorize]
public sealed class CouponsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public CouponsController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    /// <summary>Validate a coupon code before checkout.</summary>
    [HttpGet("validate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ValidateCoupon(
        [FromQuery] string code,
        [FromQuery] int subtotal,
        [FromQuery] int orderType,
        [FromQuery] Guid restaurantId,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new ValidateCouponQuery(code, userId, subtotal, (OrderType)orderType, restaurantId), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }
}
