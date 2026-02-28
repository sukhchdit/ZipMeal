namespace SwiggyClone.Application.Features.Admin.DTOs;

public sealed record RevenueDto(
    long Today,
    long ThisWeek,
    long ThisMonth,
    long AllTime);
