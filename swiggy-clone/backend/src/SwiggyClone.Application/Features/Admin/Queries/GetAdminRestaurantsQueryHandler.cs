using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Admin.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Admin.Queries;

internal sealed class GetAdminRestaurantsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetAdminRestaurantsQuery, Result<PagedResult<AdminRestaurantDto>>>
{
    public async Task<Result<PagedResult<AdminRestaurantDto>>> Handle(
        GetAdminRestaurantsQuery request, CancellationToken ct)
    {
        var query = db.Restaurants
            .AsNoTracking()
            .Include(r => r.Owner)
            .AsQueryable();

        if (request.StatusFilter.HasValue)
            query = query.Where(r => r.Status == request.StatusFilter.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var pattern = $"%{request.Search.Trim()}%";
            query = query.Where(r =>
                EF.Functions.Like(r.Name, pattern) ||
                (r.City != null && EF.Functions.Like(r.City, pattern)) ||
                EF.Functions.Like(r.Owner.FullName, pattern));
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new AdminRestaurantDto(
                r.Id,
                r.Name,
                r.Slug,
                r.Description,
                r.City,
                r.State,
                r.Owner.FullName,
                r.Owner.PhoneNumber,
                r.LogoUrl,
                r.Status,
                r.StatusReason,
                r.AverageRating,
                r.TotalRatings,
                r.IsAcceptingOrders,
                r.FssaiLicense,
                r.GstNumber,
                r.CreatedAt))
            .ToListAsync(ct);

        return Result<PagedResult<AdminRestaurantDto>>.Success(
            new PagedResult<AdminRestaurantDto>(items, totalCount, request.Page, request.PageSize));
    }
}
