using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.PlatformConfig.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.PlatformConfig.Queries;

internal sealed class GetPlatformConfigQueryHandler(IAppDbContext db)
    : IRequestHandler<GetPlatformConfigQuery, Result<PlatformConfigDto>>
{
    public async Task<Result<PlatformConfigDto>> Handle(GetPlatformConfigQuery request, CancellationToken ct)
    {
        var config = await db.PlatformConfigs.AsNoTracking().FirstOrDefaultAsync(ct);

        if (config is null)
        {
            // Return sensible defaults if no row exists
            return Result<PlatformConfigDto>.Success(new PlatformConfigDto(
                Guid.Empty, 4900, 1500, 5.00m, null, DateTimeOffset.UtcNow));
        }

        var dto = new PlatformConfigDto(
            config.Id,
            config.DeliveryFeePaise,
            config.PackagingChargePaise,
            config.TaxRatePercent,
            config.FreeDeliveryThresholdPaise,
            config.UpdatedAt);

        return Result<PlatformConfigDto>.Success(dto);
    }
}
