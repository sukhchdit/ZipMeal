using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.PlatformConfig.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.PlatformConfig.Commands;

internal sealed class UpdatePlatformConfigCommandHandler(IAppDbContext db)
    : IRequestHandler<UpdatePlatformConfigCommand, Result<PlatformConfigDto>>
{
    public async Task<Result<PlatformConfigDto>> Handle(UpdatePlatformConfigCommand request, CancellationToken ct)
    {
        var config = await db.PlatformConfigs.FirstOrDefaultAsync(ct);

        if (config is null)
        {
            // Create the singleton row if it doesn't exist
            config = Domain.Entities.PlatformConfig.Create(
                request.DeliveryFeePaise,
                request.PackagingChargePaise,
                request.TaxRatePercent,
                request.FreeDeliveryThresholdPaise);

            db.PlatformConfigs.Add(config);
        }
        else
        {
            config.Update(
                request.DeliveryFeePaise,
                request.PackagingChargePaise,
                request.TaxRatePercent,
                request.FreeDeliveryThresholdPaise);
        }

        await db.SaveChangesAsync(ct);

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
