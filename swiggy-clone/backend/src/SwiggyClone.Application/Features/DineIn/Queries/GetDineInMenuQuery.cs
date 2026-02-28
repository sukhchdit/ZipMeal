using MediatR;
using SwiggyClone.Application.Features.Discovery.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Queries;

public sealed record GetDineInMenuQuery(
    Guid UserId,
    Guid SessionId) : IRequest<Result<PublicRestaurantDetailDto>>;
