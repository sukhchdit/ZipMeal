using MediatR;
using SwiggyClone.Application.Features.DineIn.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Commands;

public sealed record CreateTableCommand(
    Guid OwnerId,
    Guid RestaurantId,
    string TableNumber,
    int Capacity,
    string? FloorSection) : IRequest<Result<RestaurantTableDetailDto>>;
