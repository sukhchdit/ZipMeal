using MediatR;
using SwiggyClone.Application.Features.DineIn.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Queries;

public sealed record GetRestaurantTablesQuery(
    Guid RestaurantId,
    Guid OwnerId) : IRequest<Result<List<RestaurantTableDetailDto>>>;
