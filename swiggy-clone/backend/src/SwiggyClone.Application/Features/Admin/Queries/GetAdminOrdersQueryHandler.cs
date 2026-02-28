using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Admin.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Admin.Queries;

internal sealed class GetAdminOrdersQueryHandler(IAppDbContext db)
    : IRequestHandler<GetAdminOrdersQuery, Result<PagedResult<AdminOrderSummaryDto>>>
{
    public async Task<Result<PagedResult<AdminOrderSummaryDto>>> Handle(
        GetAdminOrdersQuery request, CancellationToken ct)
    {
        var query = db.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.Restaurant)
            .AsQueryable();

        if (request.StatusFilter.HasValue)
            query = query.Where(o => o.Status == request.StatusFilter.Value);

        if (request.FromDate.HasValue)
            query = query.Where(o => o.CreatedAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(o => o.CreatedAt <= request.ToDate.Value);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(o => new AdminOrderSummaryDto(
                o.Id,
                o.OrderNumber,
                o.User.FullName,
                o.Restaurant.Name,
                o.Status,
                o.PaymentStatus,
                o.TotalAmount,
                o.CreatedAt))
            .ToListAsync(ct);

        return Result<PagedResult<AdminOrderSummaryDto>>.Success(
            new PagedResult<AdminOrderSummaryDto>(items, totalCount, request.Page, request.PageSize));
    }
}
