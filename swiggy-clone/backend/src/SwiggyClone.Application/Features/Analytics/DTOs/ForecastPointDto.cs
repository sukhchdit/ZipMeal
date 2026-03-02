namespace SwiggyClone.Application.Features.Analytics.DTOs;

public sealed record ForecastPointDto(
    string Label,
    decimal PredictedValue,
    decimal LowerBound,
    decimal UpperBound);
