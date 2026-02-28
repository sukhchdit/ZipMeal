using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Api.Contracts.Restaurants;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Analytics.Queries;
using SwiggyClone.Application.Features.Restaurants.Commands;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Application.Features.Restaurants.Queries;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[Route("api/v1/restaurants")]
[Authorize(Policy = "RestaurantOwner")]
public sealed class RestaurantsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public RestaurantsController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    // ──────────────────────────────────────────────
    //  Restaurant CRUD
    // ──────────────────────────────────────────────

    /// <summary>Register a new restaurant.</summary>
    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterRestaurant(
        [FromBody] RegisterRestaurantRequest request, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new RegisterRestaurantCommand(
            ownerId,
            request.Name,
            request.Description,
            request.PhoneNumber,
            request.Email,
            request.AddressLine1,
            request.AddressLine2,
            request.City,
            request.State,
            request.PostalCode,
            request.Latitude,
            request.Longitude,
            request.IsVegOnly,
            request.AvgCostForTwo,
            request.CuisineIds), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get all restaurants owned by the current user.</summary>
    [HttpGet("my")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyRestaurants(CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetMyRestaurantsQuery(ownerId), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get a restaurant by ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRestaurantById(Guid id, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetRestaurantByIdQuery(id, ownerId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Update restaurant profile.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateRestaurantProfile(
        Guid id, [FromBody] UpdateRestaurantRequest request, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new UpdateRestaurantProfileCommand(
            id,
            ownerId,
            request.Name,
            request.Description,
            request.PhoneNumber,
            request.Email,
            request.AddressLine1,
            request.AddressLine2,
            request.City,
            request.State,
            request.PostalCode,
            request.Latitude,
            request.Longitude,
            request.IsVegOnly,
            request.AvgCostForTwo,
            request.CuisineIds), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Toggle accepting orders.</summary>
    [HttpPut("{id:guid}/accepting-orders")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ToggleAcceptingOrders(
        Guid id, [FromBody] ToggleRequest request, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new ToggleAcceptingOrdersCommand(id, ownerId, request.Value), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Toggle dine-in availability.</summary>
    [HttpPut("{id:guid}/dine-in")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ToggleDineIn(
        Guid id, [FromBody] ToggleRequest request, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new ToggleDineInCommand(id, ownerId, request.Value), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get restaurant dashboard summary.</summary>
    [HttpGet("{id:guid}/dashboard")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRestaurantDashboard(Guid id, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetRestaurantDashboardQuery(id, ownerId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get analytics for a restaurant.</summary>
    [HttpGet("{id:guid}/analytics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRestaurantAnalytics(
        Guid id,
        [FromQuery] string period = "daily",
        [FromQuery] int days = 30,
        CancellationToken ct = default)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new GetRestaurantAnalyticsQuery(id, ownerId, period, days), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Upload a file for the restaurant (logo, banner, etc.).</summary>
    [HttpPost("{id:guid}/upload/{fileType}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadRestaurantFile(
        Guid id, string fileType, [FromForm] IFormFile file, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new UploadRestaurantFileCommand(
            id, ownerId, fileType, file.OpenReadStream(), file.FileName), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    // ──────────────────────────────────────────────
    //  Menu Categories
    // ──────────────────────────────────────────────

    /// <summary>Get all menu categories for a restaurant.</summary>
    [HttpGet("{id:guid}/menu-categories")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMenuCategories(Guid id, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetMenuCategoriesQuery(id, ownerId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Create a new menu category.</summary>
    [HttpPost("{id:guid}/menu-categories")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateMenuCategory(
        Guid id, [FromBody] CreateMenuCategoryRequest request, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new CreateMenuCategoryCommand(
            id, ownerId, request.Name, request.Description, request.SortOrder), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Update an existing menu category.</summary>
    [HttpPut("{id:guid}/menu-categories/{categoryId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMenuCategory(
        Guid id, Guid categoryId, [FromBody] UpdateMenuCategoryRequest request, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new UpdateMenuCategoryCommand(
            id, ownerId, categoryId, request.Name, request.Description, request.SortOrder, request.IsActive), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Delete a menu category.</summary>
    [HttpDelete("{id:guid}/menu-categories/{categoryId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteMenuCategory(
        Guid id, Guid categoryId, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new DeleteMenuCategoryCommand(id, ownerId, categoryId), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    // ──────────────────────────────────────────────
    //  Menu Items
    // ──────────────────────────────────────────────

    /// <summary>Get menu items by category.</summary>
    [HttpGet("{id:guid}/menu-categories/{categoryId:guid}/items")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMenuItemsByCategory(
        Guid id, Guid categoryId, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetMenuItemsByCategoryQuery(id, ownerId, categoryId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get a single menu item by ID.</summary>
    [HttpGet("{id:guid}/menu-items/{itemId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMenuItemById(
        Guid id, Guid itemId, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetMenuItemByIdQuery(itemId, id, ownerId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Create a new menu item.</summary>
    [HttpPost("{id:guid}/menu-items")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateMenuItem(
        Guid id, [FromBody] CreateMenuItemRequest request, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;

        var variants = request.Variants?
            .Select(v => new CreateVariantDto(v.Name, v.PriceAdjustment, v.IsDefault, v.IsAvailable, v.SortOrder))
            .ToList();

        var addons = request.Addons?
            .Select(a => new CreateAddonDto(a.Name, a.Price, a.IsVeg, a.IsAvailable, a.MaxQuantity, a.SortOrder))
            .ToList();

        var result = await _sender.Send(new CreateMenuItemCommand(
            id, ownerId, request.CategoryId,
            request.Name, request.Description,
            request.Price, request.DiscountedPrice, request.ImageUrl,
            request.IsVeg, request.IsAvailable, request.IsBestseller,
            request.PreparationTimeMin, request.SortOrder,
            variants, addons), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Update an existing menu item.</summary>
    [HttpPut("{id:guid}/menu-items/{itemId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMenuItem(
        Guid id, Guid itemId, [FromBody] UpdateMenuItemRequest request, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new UpdateMenuItemCommand(
            id, ownerId, itemId, request.CategoryId,
            request.Name, request.Description,
            request.Price, request.DiscountedPrice, request.ImageUrl,
            request.IsVeg, request.IsAvailable, request.IsBestseller,
            request.PreparationTimeMin, request.SortOrder), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Soft-delete a menu item.</summary>
    [HttpDelete("{id:guid}/menu-items/{itemId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteMenuItem(
        Guid id, Guid itemId, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new DeleteMenuItemCommand(id, ownerId, itemId), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    // ──────────────────────────────────────────────
    //  Menu Item Variants
    // ──────────────────────────────────────────────

    /// <summary>Add a variant to a menu item.</summary>
    [HttpPost("{id:guid}/menu-items/{itemId:guid}/variants")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddMenuItemVariant(
        Guid id, Guid itemId, [FromBody] CreateVariantRequest request, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new AddMenuItemVariantCommand(
            id, ownerId, itemId,
            request.Name, request.PriceAdjustment, request.IsDefault, request.IsAvailable, request.SortOrder), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Update a menu item variant.</summary>
    [HttpPut("{id:guid}/menu-items/{itemId:guid}/variants/{vid:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMenuItemVariant(
        Guid id, Guid itemId, Guid vid, [FromBody] UpdateVariantRequest request, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new UpdateMenuItemVariantCommand(
            id, ownerId, itemId, vid,
            request.Name, request.PriceAdjustment, request.IsDefault, request.IsAvailable, request.SortOrder), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Delete a menu item variant.</summary>
    [HttpDelete("{id:guid}/menu-items/{itemId:guid}/variants/{vid:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteMenuItemVariant(
        Guid id, Guid itemId, Guid vid, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new DeleteMenuItemVariantCommand(id, ownerId, itemId, vid), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    // ──────────────────────────────────────────────
    //  Menu Item Addons
    // ──────────────────────────────────────────────

    /// <summary>Add an addon to a menu item.</summary>
    [HttpPost("{id:guid}/menu-items/{itemId:guid}/addons")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddMenuItemAddon(
        Guid id, Guid itemId, [FromBody] CreateAddonRequest request, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new AddMenuItemAddonCommand(
            id, ownerId, itemId,
            request.Name, request.Price, request.IsVeg, request.IsAvailable, request.MaxQuantity, request.SortOrder), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Update a menu item addon.</summary>
    [HttpPut("{id:guid}/menu-items/{itemId:guid}/addons/{aid:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMenuItemAddon(
        Guid id, Guid itemId, Guid aid, [FromBody] UpdateAddonRequest request, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new UpdateMenuItemAddonCommand(
            id, ownerId, itemId, aid,
            request.Name, request.Price, request.IsVeg, request.IsAvailable, request.MaxQuantity, request.SortOrder), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Delete a menu item addon.</summary>
    [HttpDelete("{id:guid}/menu-items/{itemId:guid}/addons/{aid:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteMenuItemAddon(
        Guid id, Guid itemId, Guid aid, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new DeleteMenuItemAddonCommand(id, ownerId, itemId, aid), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    // ──────────────────────────────────────────────
    //  Operating Hours
    // ──────────────────────────────────────────────

    /// <summary>Get operating hours for a restaurant.</summary>
    [HttpGet("{id:guid}/operating-hours")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOperatingHours(Guid id, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetOperatingHoursQuery(id, ownerId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Create or update operating hours for a restaurant.</summary>
    [HttpPut("{id:guid}/operating-hours")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpsertOperatingHours(
        Guid id, [FromBody] UpsertOperatingHoursRequest request, CancellationToken ct)
    {
        var ownerId = _currentUser.UserId!.Value;

        var entries = request.Hours
            .Select(h => new OperatingHourEntry(h.DayOfWeek, h.OpenTime, h.CloseTime, h.IsClosed))
            .ToList();

        var result = await _sender.Send(new UpsertOperatingHoursCommand(id, ownerId, entries), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }
}
