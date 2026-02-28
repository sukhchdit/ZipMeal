using MediatR;
using SwiggyClone.Application.Features.Deliveries.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Deliveries.Commands;

public sealed record AcceptDeliveryCommand(
    Guid PartnerId,
    Guid AssignmentId) : IRequest<Result<DeliveryAssignmentDto>>;
