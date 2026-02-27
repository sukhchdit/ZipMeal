using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Addresses.Commands;

public sealed record DeleteAddressCommand(Guid UserId, Guid AddressId) : IRequest<Result>;
