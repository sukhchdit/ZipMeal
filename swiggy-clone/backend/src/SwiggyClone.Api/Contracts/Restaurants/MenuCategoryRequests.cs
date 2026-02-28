namespace SwiggyClone.Api.Contracts.Restaurants;

public sealed record CreateMenuCategoryRequest(string Name, string? Description, int SortOrder);
public sealed record UpdateMenuCategoryRequest(string Name, string? Description, int SortOrder, bool IsActive);
