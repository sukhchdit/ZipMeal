using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Banners.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Banners.Queries;

internal sealed class GetBannersQueryHandler(IAppDbContext db)
    : IRequestHandler<GetBannersQuery, Result<PagedResult<AdminBannerDto>>>
{
    public async Task<Result<PagedResult<AdminBannerDto>>> Handle(GetBannersQuery request, CancellationToken ct)
    {
        var query = db.Banners.AsNoTracking().AsQueryable();

        if (request.IsActive.HasValue)
            query = query.Where(b => b.IsActive == request.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = $"%{request.Search}%";
            query = query.Where(b => EF.Functions.Like(b.Title, search));
        }

        query = query.OrderBy(b => b.DisplayOrder).ThenByDescending(b => b.CreatedAt);

        var totalCount = await query.CountAsync(ct);

        var banners = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(b => new AdminBannerDto(
                b.Id, b.Title, b.ImageUrl, b.DeepLink,
                b.DisplayOrder, b.ValidFrom, b.ValidUntil,
                b.IsActive, b.CreatedAt, b.UpdatedAt))
            .ToListAsync(ct);

        var result = new PagedResult<AdminBannerDto>(banners, totalCount, request.Page, request.PageSize);
        return Result<PagedResult<AdminBannerDto>>.Success(result);
    }
}
