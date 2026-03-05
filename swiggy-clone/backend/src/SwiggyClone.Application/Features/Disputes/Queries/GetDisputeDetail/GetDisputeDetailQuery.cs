using MediatR;
using SwiggyClone.Application.Features.Disputes.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Disputes.Queries.GetDisputeDetail;

public sealed record GetDisputeDetailQuery(
    Guid UserId,
    Guid DisputeId) : IRequest<Result<DisputeDto>>;
