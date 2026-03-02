using MediatR;
using SwiggyClone.Application.Features.Analytics.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Analytics.Queries;

public sealed record GetCustomerFunnelQuery(int Days)
    : IRequest<Result<CustomerFunnelDto>>;
