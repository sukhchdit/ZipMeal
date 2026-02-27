using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Banners.Commands;

internal sealed class ToggleBannerCommandHandler(IAppDbContext db)
    : IRequestHandler<ToggleBannerCommand, Result>
{
    public async Task<Result> Handle(ToggleBannerCommand request, CancellationToken ct)
    {
        var banner = await db.Banners.FirstOrDefaultAsync(b => b.Id == request.Id, ct);
        if (banner is null)
            return Result.Failure("BANNER_NOT_FOUND", "Banner not found.");

        banner.IsActive = request.IsActive;
        banner.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
