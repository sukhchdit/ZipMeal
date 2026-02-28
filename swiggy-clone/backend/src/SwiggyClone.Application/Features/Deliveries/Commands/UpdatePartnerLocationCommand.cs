using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Deliveries.Commands;

public sealed record UpdatePartnerLocationCommand(
    Guid PartnerId,
    double Latitude,
    double Longitude,
    double? Heading,
    double? Speed) : IRequest<Result>;
