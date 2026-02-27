using MediatR;
using SwiggyClone.Application.Features.Addresses.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Addresses.Commands;

public sealed record UpdateAddressCommand(
    Guid UserId,
    Guid AddressId,
    string Label,
    string AddressLine1,
    string? AddressLine2,
    string? City,
    string? State,
    string? PostalCode,
    string? Country,
    double Latitude,
    double Longitude) : IRequest<Result<AddressDto>>;
