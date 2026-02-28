using MediatR;
using SwiggyClone.Application.Features.Admin.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Admin.Queries;

public sealed record GetAdminRestaurantDetailQuery(Guid RestaurantId) : IRequest<Result<AdminRestaurantDto>>;
