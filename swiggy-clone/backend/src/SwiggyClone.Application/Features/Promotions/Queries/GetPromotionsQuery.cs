using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Promotions.Dtos;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Promotions.Queries;

public sealed record GetPromotionsQuery(
    Guid? OwnerId,
    PromotionType? PromotionType,
    bool? IsActive,
    string? Search,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<PromotionListDto>>>;

internal sealed class GetPromotionsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetPromotionsQuery, Result<PagedResult<PromotionListDto>>>
{
    public async Task<Result<PagedResult<PromotionListDto>>> Handle(
        GetPromotionsQuery request, CancellationToken ct)
    {
        var query = db.RestaurantPromotions.AsNoTracking().AsQueryable();

        if (request.OwnerId.HasValue)
        {
            var restaurant = await db.Restaurants
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.OwnerId == request.OwnerId.Value, ct);

            if (restaurant is null)
                return Result<PagedResult<PromotionListDto>>.Failure(
                    "RESTAURANT_NOT_FOUND", "Restaurant not found.");

            query = query.Where(p => p.RestaurantId == restaurant.Id);
        }

        if (request.PromotionType.HasValue)
            query = query.Where(p => p.PromotionType == request.PromotionType.Value);

        if (request.IsActive.HasValue)
            query = query.Where(p => p.IsActive == request.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = $"%{request.Search}%";
            query = query.Where(p => EF.Functions.Like(p.Title, search));
        }

        query = query.OrderBy(p => p.DisplayOrder).ThenByDescending(p => p.CreatedAt);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PromotionListDto(
                p.Id,
                p.RestaurantId,
                p.Title,
                (short)p.PromotionType,
                (short)p.DiscountType,
                p.DiscountValue,
                p.MaxDiscount,
                p.ValidFrom,
                p.ValidUntil,
                p.IsActive,
                p.DisplayOrder,
                p.ComboPrice,
                p.PromotionMenuItems.Count,
                p.CreatedAt))
            .ToListAsync(ct);

        var result = new PagedResult<PromotionListDto>(items, totalCount, request.Page, request.PageSize);
        return Result<PagedResult<PromotionListDto>>.Success(result);
    }
}
