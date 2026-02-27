using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Api.Contracts.Addresses;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Addresses.Commands;
using SwiggyClone.Application.Features.Addresses.Queries;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[Route("api/v1/users/addresses")]
[Authorize]
public sealed class AddressesController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public AddressesController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    /// <summary>Get all addresses for the current user.</summary>
    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAddresses(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetAddressesQuery(userId), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get an address by ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAddress(Guid id, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetAddressByIdQuery(userId, id), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Create a new address.</summary>
    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAddress(
        [FromBody] CreateAddressRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var command = new CreateAddressCommand(
            userId,
            request.Label,
            request.AddressLine1,
            request.AddressLine2,
            request.City,
            request.State,
            request.PostalCode,
            request.Country,
            request.Latitude,
            request.Longitude,
            request.IsDefault);

        var result = await _sender.Send(command, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAddress), new { id = result.Value.Id }, result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Update an existing address.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAddress(
        Guid id,
        [FromBody] UpdateAddressRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var command = new UpdateAddressCommand(
            userId,
            id,
            request.Label,
            request.AddressLine1,
            request.AddressLine2,
            request.City,
            request.State,
            request.PostalCode,
            request.Country,
            request.Latitude,
            request.Longitude);

        var result = await _sender.Send(command, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Delete an address (soft-delete).</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAddress(Guid id, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new DeleteAddressCommand(userId, id), ct);
        return result.IsSuccess
            ? NoContent()
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Set an address as the default.</summary>
    [HttpPut("{id:guid}/default")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetDefault(Guid id, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new SetDefaultAddressCommand(userId, id), ct);
        return result.IsSuccess
            ? Ok()
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }
}
