using MediatR;
using SwiggyClone.Application.Features.PlatformConfig.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.PlatformConfig.Commands;

public sealed record UpdatePlatformConfigCommand(
    int DeliveryFeePaise,
    int PackagingChargePaise,
    decimal TaxRatePercent,
    int? FreeDeliveryThresholdPaise) : IRequest<Result<PlatformConfigDto>>;
