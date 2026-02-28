using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Api.Authorization;
using SwiggyClone.Api.Contracts.Admin;
using SwiggyClone.Application.Features.Admin.Commands;
using SwiggyClone.Application.Features.Admin.Queries;
using SwiggyClone.Application.Features.Analytics.Queries;
using SwiggyClone.Api.Contracts.Banners;
using SwiggyClone.Api.Contracts.Config;
using SwiggyClone.Api.Contracts.Coupons;
using SwiggyClone.Application.Features.Banners.Commands;
using SwiggyClone.Application.Features.Banners.Queries;
using SwiggyClone.Application.Features.Coupons.Commands;
using SwiggyClone.Application.Features.Coupons.Queries;
using SwiggyClone.Application.Features.Discovery.Commands;
using SwiggyClone.Application.Features.PlatformConfig.Commands;
using SwiggyClone.Application.Features.PlatformConfig.Queries;
using SwiggyClone.Api.Contracts.Subscriptions;
using SwiggyClone.Application.Features.Subscriptions.Commands.CreatePlan;
using SwiggyClone.Application.Features.Subscriptions.Commands.UpdatePlan;
using SwiggyClone.Application.Features.Subscriptions.Commands.TogglePlan;
using SwiggyClone.Application.Features.Subscriptions.Queries.GetPlans;
using SwiggyClone.Api.Security;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[AdminIpWhitelist]
public sealed class AdminController : ControllerBase
{
    private readonly ISender _sender;

    public AdminController(ISender sender)
    {
        _sender = sender;
    }

    // ─────────────────────── Dashboard ─────────────────────────

    /// <summary>Get platform-wide admin dashboard stats.</summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard(CancellationToken ct)
    {
        var result = await _sender.Send(new GetAdminDashboardQuery(), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    // ─────────────────────── Users ─────────────────────────────

    /// <summary>List all users with search, role filter, and pagination.</summary>
    [HttpGet("users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] string? search,
        [FromQuery] UserRole? role,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new GetAdminUsersQuery(search, role, page, pageSize), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get a single user's details.</summary>
    [HttpGet("users/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserDetail(Guid userId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetAdminUserDetailQuery(userId), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Toggle a user's active/inactive status.</summary>
    [HttpPut("users/{userId:guid}/active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleUserActive(
        Guid userId,
        [FromBody] ToggleUserActiveRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new ToggleUserActiveCommand(userId, request.IsActive), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Change a user's role.</summary>
    [HttpPut("users/{userId:guid}/role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeUserRole(
        Guid userId,
        [FromBody] ChangeUserRoleRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new ChangeUserRoleCommand(userId, (UserRole)request.NewRole), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    // ─────────────────────── Restaurants ───────────────────────

    /// <summary>List all restaurants with status filter, search, and pagination.</summary>
    [HttpGet("restaurants")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRestaurants(
        [FromQuery] RestaurantStatus? status,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new GetAdminRestaurantsQuery(status, search, page, pageSize), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get a single restaurant's details.</summary>
    [HttpGet("restaurants/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRestaurantDetail(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new GetAdminRestaurantDetailQuery(id), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Approve a pending restaurant.</summary>
    [HttpPut("restaurants/{id:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ApproveRestaurant(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new ApproveRestaurantCommand(id), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Reject a pending restaurant with a reason.</summary>
    [HttpPut("restaurants/{id:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RejectRestaurant(
        Guid id,
        [FromBody] RejectRestaurantRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(new RejectRestaurantCommand(id, request.Reason), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Suspend an approved restaurant with a reason.</summary>
    [HttpPut("restaurants/{id:guid}/suspend")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SuspendRestaurant(
        Guid id,
        [FromBody] SuspendRestaurantRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(new SuspendRestaurantCommand(id, request.Reason), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Reactivate a suspended restaurant.</summary>
    [HttpPut("restaurants/{id:guid}/reactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReactivateRestaurant(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new ReactivateRestaurantCommand(id), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    // ─────────────────────── Orders ────────────────────────────

    /// <summary>List all orders with status/date filter and pagination.</summary>
    [HttpGet("orders")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrders(
        [FromQuery] OrderStatus? status,
        [FromQuery] DateTimeOffset? fromDate,
        [FromQuery] DateTimeOffset? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new GetAdminOrdersQuery(status, fromDate, toDate, page, pageSize), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get a single order's full details including items.</summary>
    [HttpGet("orders/{orderId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderDetail(Guid orderId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetAdminOrderDetailQuery(orderId), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    // ─────────────────────── Coupons ────────────────────────────

    /// <summary>List all coupons with search and pagination.</summary>
    [HttpGet("coupons")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCoupons(
        [FromQuery] string? search,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new GetCouponsQuery(search, isActive, page, pageSize), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Create a new coupon.</summary>
    [HttpPost("coupons")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCoupon(
        [FromBody] CreateCouponRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(new CreateCouponCommand(
            request.Code, request.Title, request.Description,
            request.DiscountType, request.DiscountValue, request.MaxDiscount,
            request.MinOrderAmount, request.ValidFrom, request.ValidUntil,
            request.MaxUsages, request.MaxUsagesPerUser,
            request.ApplicableOrderTypes ?? [], request.RestaurantIds), ct);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Update a coupon's mutable fields.</summary>
    [HttpPut("coupons/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCoupon(
        Guid id,
        [FromBody] UpdateCouponRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(new UpdateCouponCommand(
            id, request.Title, request.Description,
            request.MaxDiscount, request.MinOrderAmount,
            request.ValidFrom, request.ValidUntil,
            request.MaxUsages, request.MaxUsagesPerUser,
            request.ApplicableOrderTypes ?? [], request.RestaurantIds), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Toggle a coupon's active status.</summary>
    [HttpPut("coupons/{id:guid}/toggle")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleCoupon(
        Guid id,
        [FromBody] ToggleCouponRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(new ToggleCouponCommand(id, request.IsActive), ct);
        return result.IsSuccess
            ? Ok()
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    // ─────────────────────── Banners ─────────────────────────────

    /// <summary>List all banners with search and pagination.</summary>
    [HttpGet("banners")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBanners(
        [FromQuery] string? search,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new GetBannersQuery(search, isActive, page, pageSize), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Create a new banner.</summary>
    [HttpPost("banners")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBanner(
        [FromBody] CreateBannerRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(new CreateBannerCommand(
            request.Title, request.ImageUrl, request.DeepLink,
            request.DisplayOrder, request.ValidFrom, request.ValidUntil), ct);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Update a banner's fields.</summary>
    [HttpPut("banners/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBanner(
        Guid id,
        [FromBody] UpdateBannerRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(new UpdateBannerCommand(
            id, request.Title, request.ImageUrl, request.DeepLink,
            request.DisplayOrder, request.ValidFrom, request.ValidUntil), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Toggle a banner's active status.</summary>
    [HttpPut("banners/{id:guid}/toggle")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleBanner(
        Guid id,
        [FromBody] ToggleBannerRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(new ToggleBannerCommand(id, request.IsActive), ct);
        return result.IsSuccess
            ? Ok()
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    // ─────────────────────── Subscription Plans ───────────────────

    /// <summary>List all subscription plans with search and pagination.</summary>
    [HttpGet("subscription-plans")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubscriptionPlans(
        [FromQuery] string? search,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new GetPlansQuery(search, isActive, page, pageSize), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Create a new subscription plan.</summary>
    [HttpPost("subscription-plans")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSubscriptionPlan(
        [FromBody] CreatePlanRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(new CreatePlanCommand(
            request.Name, request.Description, request.PricePaise, request.DurationDays,
            request.FreeDelivery, request.ExtraDiscountPercent, request.NoSurgeFee), ct);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Update a subscription plan's fields.</summary>
    [HttpPut("subscription-plans/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSubscriptionPlan(
        Guid id,
        [FromBody] UpdatePlanRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(new UpdatePlanCommand(
            id, request.Name, request.Description, request.PricePaise, request.DurationDays,
            request.FreeDelivery, request.ExtraDiscountPercent, request.NoSurgeFee), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Toggle a subscription plan's active status.</summary>
    [HttpPut("subscription-plans/{id:guid}/toggle")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleSubscriptionPlan(
        Guid id,
        [FromBody] TogglePlanRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(new TogglePlanCommand(id, request.IsActive), ct);
        return result.IsSuccess
            ? Ok()
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    // ─────────────────────── Platform Config ──────────────────────

    /// <summary>Get the current platform configuration (fees, tax rate).</summary>
    [HttpGet("config")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlatformConfig(CancellationToken ct)
    {
        var result = await _sender.Send(new GetPlatformConfigQuery(), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Update the platform configuration (fees, tax rate).</summary>
    [HttpPut("config")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePlatformConfig(
        [FromBody] UpdatePlatformConfigRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(new UpdatePlatformConfigCommand(
            request.DeliveryFeePaise, request.PackagingChargePaise,
            request.TaxRatePercent, request.FreeDeliveryThresholdPaise), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    // ─────────────────────── Analytics ──────────────────────────

    /// <summary>Get platform-wide analytics with time series data.</summary>
    [HttpGet("analytics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPlatformAnalytics(
        [FromQuery] string period = "daily",
        [FromQuery] int days = 30,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new GetPlatformAnalyticsQuery(period, days), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    // ─────────────────────── Search ─────────────────────────────

    /// <summary>Trigger a full reindex of restaurants and menu items into Elasticsearch.</summary>
    [HttpPost("reindex")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Reindex(
        [FromQuery] bool recreate = false,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(new ReindexCommand(recreate), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }
}
