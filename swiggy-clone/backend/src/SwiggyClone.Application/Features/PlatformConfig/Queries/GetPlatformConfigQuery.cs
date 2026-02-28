using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.PlatformConfig.DTOs;
using SwiggyClone.Shared;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Application.Features.PlatformConfig.Queries;

public sealed record GetPlatformConfigQuery : IRequest<Result<PlatformConfigDto>>, ICacheable
{
    public string CacheKey => CacheKeys.PlatformConfigKey;
    public int CacheExpirationMinutes => 30;
}
