using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Auth.Commands;

internal sealed class LogoutAllCommandHandler(IAppDbContext db)
    : IRequestHandler<LogoutAllCommand, Result>
{
    public async Task<Result> Handle(LogoutAllCommand request, CancellationToken ct)
    {
        var activeTokens = await db.RefreshTokens
            .Where(rt => rt.UserId == request.UserId && rt.RevokedAt == null)
            .ToListAsync(ct);

        foreach (var token in activeTokens)
            token.RevokedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
