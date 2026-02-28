using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Api.Contracts.Wallet;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Wallet.Commands.AddMoney;
using SwiggyClone.Application.Features.Wallet.Queries.GetWalletBalance;
using SwiggyClone.Application.Features.Wallet.Queries.GetWalletTransactions;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[Route("api/v1/wallet")]
[Authorize]
public sealed class WalletController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public WalletController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    /// <summary>Get the current user's wallet balance.</summary>
    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalance(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetWalletBalanceQuery(userId), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Add money to the current user's wallet.</summary>
    [HttpPost("add-money")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddMoney(
        [FromBody] AddMoneyRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new AddMoneyCommand(userId, request.AmountPaise), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get the current user's wallet transaction history.</summary>
    [HttpGet("transactions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new GetWalletTransactionsQuery(userId, page, pageSize), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }
}
