using MediatR;
using SwiggyClone.Application.Features.Loyalty.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Loyalty.Queries.GetDashboard;

public sealed record GetLoyaltyDashboardQuery(Guid UserId) : IRequest<Result<LoyaltyDashboardDto>>;
