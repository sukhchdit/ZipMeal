namespace SwiggyClone.Application.Features.Restaurants.DTOs;

public sealed record MenuCategoryDto(
    Guid Id,
    string Name,
    string? Description,
    int SortOrder,
    bool IsActive,
    int ItemCount);
