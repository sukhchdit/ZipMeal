using MediatR;
using SwiggyClone.Application.Features.DineIn.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Commands;

public sealed record UpdateTableCommand(
    Guid OwnerId,
    Guid RestaurantId,
    Guid TableId,
    string? TableNumber,
    int? Capacity,
    string? FloorSection,
    bool? IsActive,
    TableStatus? Status) : IRequest<Result<RestaurantTableDetailDto>>;
