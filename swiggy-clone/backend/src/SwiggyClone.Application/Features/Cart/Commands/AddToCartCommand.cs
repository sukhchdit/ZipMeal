using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Cart.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Cart.Commands;

public sealed record AddToCartCommand(
    Guid UserId,
    Guid RestaurantId,
    Guid MenuItemId,
    Guid? VariantId,
    int Quantity,
    string? SpecialInstructions,
    List<CartAddonInput> Addons) : IRequest<Result<CartDto>>;
