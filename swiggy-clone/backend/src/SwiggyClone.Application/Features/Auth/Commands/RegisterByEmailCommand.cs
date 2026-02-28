using MediatR;
using SwiggyClone.Application.Features.Auth.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Auth.Commands;

public sealed record RegisterByEmailCommand(
    string Email,
    string Password,
    string FullName,
    string PhoneNumber,
    string? ReferralCode = null) : IRequest<Result<AuthResponse>>;
