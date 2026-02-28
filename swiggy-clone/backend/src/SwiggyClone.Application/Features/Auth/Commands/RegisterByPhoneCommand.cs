using MediatR;
using SwiggyClone.Application.Features.Auth.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Auth.Commands;

public sealed record RegisterByPhoneCommand(
    string PhoneNumber,
    string Otp,
    string FullName) : IRequest<Result<AuthResponse>>;
