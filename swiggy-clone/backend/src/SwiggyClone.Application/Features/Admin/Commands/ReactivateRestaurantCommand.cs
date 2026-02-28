using MediatR;
using SwiggyClone.Application.Features.Admin.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Admin.Commands;

public sealed record ReactivateRestaurantCommand(
    Guid RestaurantId) : IRequest<Result<AdminRestaurantDto>>;
