using MediatR;
using SwiggyClone.Application.Features.Auth.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Auth.Commands;

public sealed record UpdateProfileCommand(
    Guid UserId,
    string? FullName,
    string? Email,
    string? AvatarUrl) : IRequest<Result<UserDto>>;
