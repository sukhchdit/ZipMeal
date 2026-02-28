using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Auth.Commands;

internal sealed class RevokeSessionCommandHandler(IAppDbContext db)
    : IRequestHandler<RevokeSessionCommand, Result>
{
    public async Task<Result> Handle(RevokeSessionCommand request, CancellationToken ct)
    {
        var token = await db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Id == request.SessionId && rt.UserId == request.UserId, ct);

        if (token is null)
            return Result.Failure("SESSION_NOT_FOUND", "Session not found.");

        if (token.RevokedAt is not null)
            return Result.Failure("SESSION_ALREADY_REVOKED", "This session has already been revoked.");

        token.RevokedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
