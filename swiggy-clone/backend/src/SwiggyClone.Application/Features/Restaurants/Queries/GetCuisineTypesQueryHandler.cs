using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Queries;

internal sealed class GetCuisineTypesQueryHandler(IAppDbContext db)
    : IRequestHandler<GetCuisineTypesQuery, Result<List<CuisineTypeDto>>>
{
    public async Task<Result<List<CuisineTypeDto>>> Handle(
        GetCuisineTypesQuery request, CancellationToken ct)
    {
        var cuisines = await db.CuisineTypes
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .Select(c => new CuisineTypeDto(
                c.Id,
                c.Name,
                c.IconUrl))
            .ToListAsync(ct);

        return Result<List<CuisineTypeDto>>.Success(cuisines);
    }
}
