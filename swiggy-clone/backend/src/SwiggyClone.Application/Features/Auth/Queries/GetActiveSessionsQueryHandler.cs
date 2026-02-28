using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Auth.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Auth.Queries;

internal sealed class GetActiveSessionsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetActiveSessionsQuery, Result<List<SessionDto>>>
{
    public async Task<Result<List<SessionDto>>> Handle(
        GetActiveSessionsQuery request, CancellationToken ct)
    {
        var sessions = await db.RefreshTokens
            .AsNoTracking()
            .Where(rt => rt.UserId == request.UserId
                         && rt.RevokedAt == null
                         && rt.ExpiresAt > DateTimeOffset.UtcNow)
            .OrderByDescending(rt => rt.CreatedAt)
            .Select(rt => new SessionDto(
                rt.Id,
                rt.DeviceInfo,
                rt.CreatedAt,
                rt.ExpiresAt,
                request.CurrentTokenHash != null && rt.TokenHash == request.CurrentTokenHash))
            .ToListAsync(ct);

        return Result<List<SessionDto>>.Success(sessions);
    }
}
