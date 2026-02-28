using MediatR;
using SwiggyClone.Application.Features.Orders.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Commands;

public sealed record PlaceDineInOrderCommand(
    Guid UserId,
    Guid SessionId,
    List<DineInOrderItemInput> Items,
    string? SpecialInstructions) : IRequest<Result<OrderDto>>;

public sealed record DineInOrderItemInput(
    Guid MenuItemId,
    Guid? VariantId,
    int Quantity,
    string? SpecialInstructions,
    List<DineInOrderAddonInput> Addons);

public sealed record DineInOrderAddonInput(Guid AddonId, int Quantity = 1);
