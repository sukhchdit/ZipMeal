using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Banners.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Banners.Commands;

internal sealed class UpdateBannerCommandHandler(IAppDbContext db)
    : IRequestHandler<UpdateBannerCommand, Result<AdminBannerDto>>
{
    public async Task<Result<AdminBannerDto>> Handle(UpdateBannerCommand request, CancellationToken ct)
    {
        var banner = await db.Banners.FirstOrDefaultAsync(b => b.Id == request.Id, ct);
        if (banner is null)
            return Result<AdminBannerDto>.Failure("BANNER_NOT_FOUND", "Banner not found.");

        banner.Update(
            request.Title,
            request.ImageUrl,
            request.DeepLink,
            request.DisplayOrder,
            request.ValidFrom,
            request.ValidUntil);

        await db.SaveChangesAsync(ct);

        var dto = new AdminBannerDto(
            banner.Id, banner.Title, banner.ImageUrl, banner.DeepLink,
            banner.DisplayOrder, banner.ValidFrom, banner.ValidUntil,
            banner.IsActive, banner.CreatedAt, banner.UpdatedAt);

        return Result<AdminBannerDto>.Success(dto);
    }
}
