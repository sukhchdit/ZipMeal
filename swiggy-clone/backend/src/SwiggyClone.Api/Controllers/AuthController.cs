using MediatR;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Api.Contracts.Auth;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Auth.Commands;
using SwiggyClone.Application.Features.Auth.Queries;
using SwiggyClone.Shared;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;
    private readonly ITokenService _tokenService;

    public AuthController(ISender sender, ICurrentUserService currentUser, ITokenService tokenService)
    {
        _sender = sender;
        _currentUser = currentUser;
        _tokenService = tokenService;
    }

    /// <summary>Register with phone + OTP.</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterByPhone(
        [FromBody] RegisterByPhoneRequest request, CancellationToken ct)
    {
        var result = await _sender.Send(new RegisterByPhoneCommand(
            request.PhoneNumber, request.Otp, request.FullName), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Register with email + password.</summary>
    [HttpPost("register/email")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterByEmail(
        [FromBody] RegisterByEmailRequest request, CancellationToken ct)
    {
        var result = await _sender.Send(new RegisterByEmailCommand(
            request.Email, request.Password, request.FullName, request.PhoneNumber), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Login with phone + OTP.</summary>
    [HttpPost("login/phone")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LoginByPhone(
        [FromBody] LoginByPhoneRequest request, CancellationToken ct)
    {
        var deviceInfo = Request.Headers.UserAgent.ToString();
        var result = await _sender.Send(new LoginByPhoneCommand(
            request.PhoneNumber, request.Otp, deviceInfo), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Login with email + password.</summary>
    [HttpPost("login/email")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LoginByEmail(
        [FromBody] LoginByEmailRequest request, CancellationToken ct)
    {
        var deviceInfo = Request.Headers.UserAgent.ToString();
        var result = await _sender.Send(new LoginByEmailCommand(
            request.Email, request.Password, deviceInfo), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Send OTP to phone number.</summary>
    [HttpPost("otp/send")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendOtp(
        [FromBody] SendOtpRequest request, CancellationToken ct)
    {
        var result = await _sender.Send(new SendOtpCommand(request.PhoneNumber), ct);
        return result.IsSuccess ? Ok(new { Message = "OTP sent successfully." }) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Verify OTP (standalone verification, not tied to login/register).</summary>
    [HttpPost("otp/verify")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyOtp(
        [FromBody] LoginByPhoneRequest request, CancellationToken ct)
    {
        // Reuse the OTP verification via the phone login flow
        var deviceInfo = Request.Headers.UserAgent.ToString();
        var result = await _sender.Send(new LoginByPhoneCommand(
            request.PhoneNumber, request.Otp, deviceInfo), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Refresh access token using refresh token.</summary>
    [HttpPost("token/refresh")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var result = await _sender.Send(new RefreshTokenCommand(request.RefreshToken), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Logout current session (revoke refresh token).</summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutRequest request, CancellationToken ct)
    {
        var result = await _sender.Send(new LogoutCommand(request.RefreshToken), ct);
        return result.IsSuccess ? Ok(new { Message = "Logged out successfully." }) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Logout from all devices (revoke all refresh tokens).</summary>
    [HttpPost("logout/all")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> LogoutAll(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new LogoutAllCommand(userId), ct);
        return result.IsSuccess ? Ok(new { Message = "All sessions revoked." }) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get current authenticated user profile.</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMe(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetCurrentUserQuery(userId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Update current user profile.</summary>
    [HttpPut("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateProfileRequest request, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new UpdateProfileCommand(
            userId, request.FullName, request.Email, request.AvatarUrl), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>List active sessions for current user.</summary>
    [HttpGet("sessions")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSessions(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;

        // Extract current refresh token hash to mark the current session
        string? currentTokenHash = null;
        var authHeader = Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(authHeader))
        {
            // We can't identify the current session by access token alone,
            // so we return all sessions without marking current
        }

        var result = await _sender.Send(new GetActiveSessionsQuery(userId, currentTokenHash), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Change password for the current user.</summary>
    [HttpPut("me/password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new ChangePasswordCommand(
            userId, request.CurrentPassword, request.NewPassword), ct);
        return result.IsSuccess ? Ok(new { Message = "Password changed successfully." }) : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Delete (soft-delete) the current user's account.</summary>
    [HttpDelete("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAccount(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new DeleteAccountCommand(userId), ct);
        return result.IsSuccess ? NoContent() : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Revoke a specific session by ID.</summary>
    [HttpDelete("sessions/{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeSession(Guid id, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new RevokeSessionCommand(userId, id), ct);
        return result.IsSuccess ? Ok(new { Message = "Session revoked." }) : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }
}
