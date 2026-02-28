using MediatR;
using SwiggyClone.Application.Features.Discovery.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Discovery.Queries;

public sealed record GetSearchSuggestionsQuery(
    string Prefix,
    string? City,
    int Limit = 10) : IRequest<Result<List<SearchSuggestionDto>>>;
