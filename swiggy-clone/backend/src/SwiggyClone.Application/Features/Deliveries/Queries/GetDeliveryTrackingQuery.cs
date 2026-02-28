using MediatR;
using SwiggyClone.Application.Features.Deliveries.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Deliveries.Queries;

public sealed record GetDeliveryTrackingQuery(Guid UserId, Guid OrderId)
    : IRequest<Result<DeliveryTrackingDto>>;
