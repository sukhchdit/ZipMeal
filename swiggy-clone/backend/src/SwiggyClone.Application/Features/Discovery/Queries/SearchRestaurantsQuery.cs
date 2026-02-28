using MediatR;
using SwiggyClone.Application.Features.Discovery.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Discovery.Queries;

public sealed record SearchRestaurantsQuery(
    string Term,
    string? City,
    int PageSize = 20) : IRequest<Result<List<CustomerRestaurantDto>>>;
