using MediatR;
using SwiggyClone.Application.Features.Cart.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Cart.Queries;

public sealed record GetCartQuery(Guid UserId) : IRequest<Result<CartDto>>;
