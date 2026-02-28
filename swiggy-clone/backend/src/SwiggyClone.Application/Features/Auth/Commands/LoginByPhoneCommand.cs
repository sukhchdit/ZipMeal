using MediatR;
using SwiggyClone.Application.Features.Auth.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Auth.Commands;

public sealed record LoginByPhoneCommand(
    string PhoneNumber,
    string Otp,
    string? DeviceInfo = null) : IRequest<Result<AuthResponse>>;
