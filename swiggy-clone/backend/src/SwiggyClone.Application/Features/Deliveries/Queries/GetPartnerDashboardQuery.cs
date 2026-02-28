using MediatR;
using SwiggyClone.Application.Features.Deliveries.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Deliveries.Queries;

public sealed record GetPartnerDashboardQuery(Guid PartnerId)
    : IRequest<Result<PartnerDashboardDto>>;
