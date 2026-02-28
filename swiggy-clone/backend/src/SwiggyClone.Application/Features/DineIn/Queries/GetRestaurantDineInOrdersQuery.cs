using MediatR;
using SwiggyClone.Application.Features.DineIn.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Queries;

public sealed record GetRestaurantDineInOrdersQuery(
    Guid RestaurantId,
    Guid OwnerId,
    DineInOrderStatus? StatusFilter) : IRequest<Result<List<OwnerDineInOrderDto>>>;
