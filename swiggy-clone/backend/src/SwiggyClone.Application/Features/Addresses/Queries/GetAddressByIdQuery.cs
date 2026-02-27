using MediatR;
using SwiggyClone.Application.Features.Addresses.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Addresses.Queries;

public sealed record GetAddressByIdQuery(Guid UserId, Guid AddressId) : IRequest<Result<AddressDto>>;
