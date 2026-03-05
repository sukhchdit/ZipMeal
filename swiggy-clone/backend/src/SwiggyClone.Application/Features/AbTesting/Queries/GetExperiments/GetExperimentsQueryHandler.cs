using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.AbTesting.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.AbTesting.Queries.GetExperiments;

internal sealed class GetExperimentsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetExperimentsQuery, Result<PagedExperimentsResult>>
{
    public async Task<Result<PagedExperimentsResult>> Handle(GetExperimentsQuery request, CancellationToken ct)
    {
        var query = db.Experiments.AsNoTracking()
            .Include(e => e.Variants)
            .AsQueryable();

        if (request.Status is not null)
        {
            var status = (ExperimentStatus)request.Status.Value;
            query = query.Where(e => e.Status == status);
        }

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 50);

        var totalCount = await query.CountAsync(ct);

        var experiments = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new ExperimentDto(
                e.Id, e.Key, e.Name, e.Description,
                (int)e.Status, e.TargetAudience,
                e.StartDate, e.EndDate, e.GoalDescription,
                e.CreatedByUserId,
                e.Variants.Select(v => new ExperimentVariantDto(
                    v.Id, v.Key, v.Name, v.AllocationPercent,
                    v.ConfigJson, v.IsControl)).ToList(),
                e.CreatedAt, e.UpdatedAt))
            .ToListAsync(ct);

        return Result<PagedExperimentsResult>.Success(
            new PagedExperimentsResult(experiments, totalCount, page, pageSize));
    }
}
