using MediatR;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Discovery.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Discovery.Queries;

internal sealed class GetSearchSuggestionsQueryHandler(
    ISearchService searchService,
    ILogger<GetSearchSuggestionsQueryHandler> logger)
    : IRequestHandler<GetSearchSuggestionsQuery, Result<List<SearchSuggestionDto>>>
{
    public async Task<Result<List<SearchSuggestionDto>>> Handle(
        GetSearchSuggestionsQuery request, CancellationToken ct)
    {
        if (!await searchService.IsAvailableAsync(ct))
            return Result<List<SearchSuggestionDto>>.Success([]);

        try
        {
            var limit = Math.Clamp(request.Limit, 1, 20);
            var suggestions = await searchService.GetSuggestionsAsync(
                request.Prefix.Trim(), request.City, limit, ct);

            return Result<List<SearchSuggestionDto>>.Success(suggestions);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Suggestion fetch failed");
            return Result<List<SearchSuggestionDto>>.Success([]);
        }
    }
}
