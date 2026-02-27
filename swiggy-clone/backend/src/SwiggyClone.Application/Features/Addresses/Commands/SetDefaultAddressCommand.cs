using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Addresses.Commands;

public sealed record SetDefaultAddressCommand(Guid UserId, Guid AddressId) : IRequest<Result>;
