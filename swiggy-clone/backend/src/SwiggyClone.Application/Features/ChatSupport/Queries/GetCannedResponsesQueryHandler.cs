using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.ChatSupport.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.ChatSupport.Queries;

internal sealed class GetCannedResponsesQueryHandler(IAppDbContext db)
    : IRequestHandler<GetCannedResponsesQuery, Result<IReadOnlyList<CannedResponseDto>>>
{
    public async Task<Result<IReadOnlyList<CannedResponseDto>>> Handle(
        GetCannedResponsesQuery request, CancellationToken ct)
    {
        var query = db.CannedResponses.AsNoTracking()
            .Where(r => r.IsActive);

        if (request.Category.HasValue)
            query = query.Where(r => r.Category == (SupportTicketCategory)request.Category.Value);

        var responses = await query
            .OrderBy(r => r.Category)
            .ThenBy(r => r.SortOrder)
            .Select(r => new CannedResponseDto(
                r.Id,
                r.Title,
                r.Content,
                (int)r.Category,
                r.SortOrder))
            .ToListAsync(ct);

        return Result<IReadOnlyList<CannedResponseDto>>.Success(responses);
    }
}
