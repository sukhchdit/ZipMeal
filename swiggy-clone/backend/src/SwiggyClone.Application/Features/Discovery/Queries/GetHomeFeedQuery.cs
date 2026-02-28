using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Discovery.DTOs;
using SwiggyClone.Shared;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Application.Features.Discovery.Queries;

public sealed record GetHomeFeedQuery(string? City) : IRequest<Result<HomeFeedDto>>, ICacheable
{
    public string CacheKey => CacheKeys.HomeFeed(City);
    public int CacheExpirationMinutes => 5;
}
