using MediatR;
using SwiggyClone.Application.Features.Addresses.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Addresses.Queries;

public sealed record GetAddressesQuery(Guid UserId) : IRequest<Result<List<AddressDto>>>;
