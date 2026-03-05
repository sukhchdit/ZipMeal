namespace SwiggyClone.Application.Features.AbTesting.DTOs;

public sealed record VariantStatsDto(
    Guid VariantId,
    string VariantKey,
    string VariantName,
    bool IsControl,
    int Exposures,
    int Conversions,
    double ConversionRate,
    double? RelativeLift,
    double? ZScore,
    double? PValue,
    bool? IsSignificant);
