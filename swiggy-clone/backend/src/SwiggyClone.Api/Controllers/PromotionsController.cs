using MediatR;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Api.Contracts.Promotions;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Promotions.Commands;
using SwiggyClone.Application.Features.Promotions.Queries;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/promotions")]
[Authorize(Policy = "RestaurantOwner")]
public sealed class PromotionsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public PromotionsController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    /// <summary>Create a new promotion.</summary>
    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePromotion(
        [FromBody] CreatePromotionRequest request, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;

        TimeOnly? recurringStart = null;
        TimeOnly? recurringEnd = null;
        if (request.RecurringStartTime is not null)
            recurringStart = TimeOnly.Parse(request.RecurringStartTime, System.Globalization.CultureInfo.InvariantCulture);
        if (request.RecurringEndTime is not null)
            recurringEnd = TimeOnly.Parse(request.RecurringEndTime, System.Globalization.CultureInfo.InvariantCulture);

        var menuItems = request.MenuItems
            .Select(m => new CreatePromotionMenuItemInput(m.MenuItemId, m.Quantity))
            .ToList();

        var result = await _sender.Send(new CreatePromotionCommand(
            ownerId,
            request.Title,
            request.Description,
            request.ImageUrl,
            (PromotionType)request.PromotionType,
            (DiscountType)request.DiscountType,
            request.DiscountValue,
            request.MaxDiscount,
            request.MinOrderAmount,
            request.ValidFrom,
            request.ValidUntil,
            request.DisplayOrder,
            recurringStart,
            recurringEnd,
            request.RecurringDaysOfWeek,
            request.ComboPrice,
            menuItems), ct);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, new { id = result.Value })
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>List own promotions (paginated, filterable).</summary>
    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPromotions(
        [FromQuery] PromotionType? promotionType,
        [FromQuery] bool? isActive,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new GetPromotionsQuery(ownerId, promotionType, isActive, search, page, pageSize), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get promotion by ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPromotionById(Guid id, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetPromotionByIdQuery(id, ownerId), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Update a promotion.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePromotion(
        Guid id, [FromBody] UpdatePromotionRequest request, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;

        TimeOnly? recurringStart = null;
        TimeOnly? recurringEnd = null;
        if (request.RecurringStartTime is not null)
            recurringStart = TimeOnly.Parse(request.RecurringStartTime, System.Globalization.CultureInfo.InvariantCulture);
        if (request.RecurringEndTime is not null)
            recurringEnd = TimeOnly.Parse(request.RecurringEndTime, System.Globalization.CultureInfo.InvariantCulture);

        var menuItems = request.MenuItems
            .Select(m => new CreatePromotionMenuItemInput(m.MenuItemId, m.Quantity))
            .ToList();

        var result = await _sender.Send(new UpdatePromotionCommand(
            id,
            ownerId,
            request.Title,
            request.Description,
            request.ImageUrl,
            (DiscountType)request.DiscountType,
            request.DiscountValue,
            request.MaxDiscount,
            request.MinOrderAmount,
            request.ValidFrom,
            request.ValidUntil,
            request.DisplayOrder,
            recurringStart,
            recurringEnd,
            request.RecurringDaysOfWeek,
            request.ComboPrice,
            menuItems), ct);

        return result.IsSuccess
            ? Ok()
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Toggle promotion active status.</summary>
    [HttpPut("{id:guid}/toggle")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TogglePromotion(
        Guid id, [FromBody] TogglePromotionRequest request, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new TogglePromotionCommand(id, request.IsActive, ownerId), ct);
        return result.IsSuccess
            ? Ok()
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Soft-delete a promotion.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePromotion(Guid id, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new DeletePromotionCommand(id, ownerId), ct);
        return result.IsSuccess
            ? NoContent()
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }
}
