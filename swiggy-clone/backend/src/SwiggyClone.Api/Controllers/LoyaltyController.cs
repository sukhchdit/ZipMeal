using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Api.Contracts.Loyalty;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Loyalty.Commands.AdjustPoints;
using SwiggyClone.Application.Features.Loyalty.Commands.RedeemReward;
using SwiggyClone.Application.Features.Loyalty.Queries.GetDashboard;
using SwiggyClone.Application.Features.Loyalty.Queries.GetRewards;
using SwiggyClone.Application.Features.Loyalty.Queries.GetTransactions;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/loyalty")]
[Authorize]
public sealed class LoyaltyController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;
    private readonly IAppDbContext _db;

    public LoyaltyController(ISender sender, ICurrentUserService currentUser, IAppDbContext db)
    {
        _sender = sender;
        _currentUser = currentUser;
        _db = db;
    }

    /// <summary>Get the current user's loyalty dashboard.</summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetLoyaltyDashboardQuery(userId), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get the current user's loyalty transaction history.</summary>
    [HttpGet("transactions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] short? type = null,
        CancellationToken ct = default)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new GetLoyaltyTransactionsQuery(userId, page, pageSize, type), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get the active rewards catalog.</summary>
    [HttpGet("rewards")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRewards(CancellationToken ct)
    {
        var result = await _sender.Send(new GetLoyaltyRewardsQuery(), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Redeem a reward.</summary>
    [HttpPost("redeem/{rewardId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RedeemReward(Guid rewardId, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new RedeemRewardCommand(userId, rewardId), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Admin: adjust points manually.</summary>
    [HttpPost("admin/adjust")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AdjustPoints(
        [FromBody] AdjustPointsRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new AdjustPointsCommand(request.UserId, request.Points, request.Description), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Admin: create a reward.</summary>
    [HttpPost("admin/rewards")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateReward(
        [FromBody] CreateRewardRequest request,
        CancellationToken ct)
    {
        var reward = new LoyaltyReward
        {
            Id = Guid.CreateVersion7(),
            Name = request.Name,
            Description = request.Description,
            PointsCost = request.PointsCost,
            RewardType = (LoyaltyRewardType)request.RewardType,
            RewardValue = request.RewardValue,
            IsActive = true,
            Stock = request.Stock,
            ExpiresAt = request.ExpiresAt,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        _db.LoyaltyRewards.Add(reward);
        await _db.SaveChangesAsync(ct);

        return Created($"/api/v1/loyalty/admin/rewards/{reward.Id}", new { reward.Id });
    }

    /// <summary>Admin: update a reward.</summary>
    [HttpPut("admin/rewards/{rewardId:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateReward(
        Guid rewardId,
        [FromBody] UpdateRewardRequest request,
        CancellationToken ct)
    {
        var reward = await _db.LoyaltyRewards
            .FirstOrDefaultAsync(r => r.Id == rewardId, ct);

        if (reward is null)
        {
            return NotFound(new { ErrorCode = "LOYALTY_REWARD_NOT_FOUND", ErrorMessage = "Reward not found." });
        }

        reward.Name = request.Name;
        reward.Description = request.Description;
        reward.PointsCost = request.PointsCost;
        reward.RewardType = (LoyaltyRewardType)request.RewardType;
        reward.RewardValue = request.RewardValue;
        reward.IsActive = request.IsActive;
        reward.Stock = request.Stock;
        reward.ExpiresAt = request.ExpiresAt;
        reward.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);

        return Ok(new { reward.Id });
    }

    /// <summary>Admin: deactivate a reward.</summary>
    [HttpDelete("admin/rewards/{rewardId:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateReward(
        Guid rewardId,
        CancellationToken ct)
    {
        var reward = await _db.LoyaltyRewards
            .FirstOrDefaultAsync(r => r.Id == rewardId, ct);

        if (reward is null)
        {
            return NotFound(new { ErrorCode = "LOYALTY_REWARD_NOT_FOUND", ErrorMessage = "Reward not found." });
        }

        reward.IsActive = false;
        reward.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);

        return NoContent();
    }
}
