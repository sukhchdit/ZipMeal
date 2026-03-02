namespace SwiggyClone.Application.Features.Analytics.DTOs;

public sealed record CustomerFunnelDto(
    List<FunnelStageDto> Stages,
    List<DataPointDto> ActiveUserTrend);
