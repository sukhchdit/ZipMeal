namespace SwiggyClone.Application.Features.Admin.DTOs;

public sealed record OrderCountsDto(
    int Today,
    int ThisWeek,
    int ThisMonth,
    int AllTime);
