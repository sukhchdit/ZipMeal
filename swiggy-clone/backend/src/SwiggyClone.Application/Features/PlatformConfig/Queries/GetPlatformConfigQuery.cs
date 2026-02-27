using MediatR;
using SwiggyClone.Application.Features.PlatformConfig.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.PlatformConfig.Queries;

public sealed record GetPlatformConfigQuery : IRequest<Result<PlatformConfigDto>>;
