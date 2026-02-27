using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Banners.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Banners.Commands;

internal sealed class CreateBannerCommandHandler(IAppDbContext db)
    : IRequestHandler<CreateBannerCommand, Result<AdminBannerDto>>
{
    public async Task<Result<AdminBannerDto>> Handle(CreateBannerCommand request, CancellationToken ct)
    {
        var banner = Banner.Create(
            request.Title,
            request.ImageUrl,
            request.DeepLink,
            request.DisplayOrder,
            request.ValidFrom,
            request.ValidUntil);

        db.Banners.Add(banner);
        await db.SaveChangesAsync(ct);

        var dto = new AdminBannerDto(
            banner.Id, banner.Title, banner.ImageUrl, banner.DeepLink,
            banner.DisplayOrder, banner.ValidFrom, banner.ValidUntil,
            banner.IsActive, banner.CreatedAt, banner.UpdatedAt);

        return Result<AdminBannerDto>.Success(dto);
    }
}
