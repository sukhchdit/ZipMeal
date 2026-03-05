using SwiggyClone.Domain.Entities;

namespace SwiggyClone.UnitTests.Common.Builders;

public sealed class MenuItemBuilder
{
    private Guid _id = TestConstants.MenuItemId;
    private Guid _categoryId = TestConstants.CategoryId;
    private Guid _restaurantId = TestConstants.RestaurantId;
    private string _name = "Test Item";
    private int _price = 19900;
    private bool _isVeg = true;
    private bool _isAvailable = true;

    public MenuItemBuilder WithId(Guid id) { _id = id; return this; }
    public MenuItemBuilder WithCategoryId(Guid categoryId) { _categoryId = categoryId; return this; }
    public MenuItemBuilder WithRestaurantId(Guid restaurantId) { _restaurantId = restaurantId; return this; }
    public MenuItemBuilder WithName(string name) { _name = name; return this; }
    public MenuItemBuilder WithPrice(int price) { _price = price; return this; }
    public MenuItemBuilder WithIsVeg(bool isVeg) { _isVeg = isVeg; return this; }
    public MenuItemBuilder WithIsAvailable(bool available) { _isAvailable = available; return this; }

    public MenuItem Build() => new()
    {
        Id = _id,
        CategoryId = _categoryId,
        RestaurantId = _restaurantId,
        Name = _name,
        Description = $"Description for {_name}",
        Price = _price,
        IsVeg = _isVeg,
        IsAvailable = _isAvailable,
        IsBestseller = false,
        PreparationTimeMin = 15,
        SortOrder = 1,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
    };
}
