using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Dietary.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Dietary.Queries;

public sealed record GetDietaryProfileQuery(Guid UserId) : IRequest<Result<DietaryProfileDto>>;

internal sealed class GetDietaryProfileQueryHandler(IAppDbContext db)
    : IRequestHandler<GetDietaryProfileQuery, Result<DietaryProfileDto>>
{
    public async Task<Result<DietaryProfileDto>> Handle(
        GetDietaryProfileQuery request, CancellationToken ct)
    {
        var profile = await db.UserDietaryProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, ct);

        var dto = profile is not null
            ? new DietaryProfileDto(
                profile.Id, profile.UserId,
                profile.AllergenAlerts, profile.DietaryPreferences, profile.MaxSpiceLevel)
            : new DietaryProfileDto(
                Guid.Empty, request.UserId, null, null, null);

        return Result<DietaryProfileDto>.Success(dto);
    }
}
