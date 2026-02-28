using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Deliveries.Commands;

public sealed record UpdateDeliveryStatusCommand(
    Guid PartnerId,
    Guid AssignmentId,
    int NewStatus) : IRequest<Result>;
