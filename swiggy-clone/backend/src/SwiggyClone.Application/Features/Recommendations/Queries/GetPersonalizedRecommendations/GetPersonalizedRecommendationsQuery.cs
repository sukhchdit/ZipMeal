using MediatR;
using SwiggyClone.Application.Features.Recommendations.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Recommendations.Queries.GetPersonalizedRecommendations;

public sealed record GetPersonalizedRecommendationsQuery(
    Guid UserId,
    string? City = null,
    int MaxRestaurants = 10,
    int MaxItems = 10) : IRequest<Result<PersonalizedRecommendationsDto>>;
