using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.UnitTests.Common.Builders;

public sealed class RestaurantBuilder
{
    private Guid _id = TestConstants.RestaurantId;
    private Guid _ownerId = TestConstants.OwnerId;
    private string _name = "Test Restaurant";
    private string _slug = TestConstants.ValidSlug;
    private RestaurantStatus _status = RestaurantStatus.Approved;

    public RestaurantBuilder WithId(Guid id) { _id = id; return this; }
    public RestaurantBuilder WithOwnerId(Guid ownerId) { _ownerId = ownerId; return this; }
    public RestaurantBuilder WithName(string name) { _name = name; return this; }
    public RestaurantBuilder WithSlug(string slug) { _slug = slug; return this; }
    public RestaurantBuilder WithStatus(RestaurantStatus status) { _status = status; return this; }

    public Restaurant Build() => new()
    {
        Id = _id,
        OwnerId = _ownerId,
        Name = _name,
        Slug = _slug,
        PhoneNumber = TestConstants.ValidPhone,
        Email = TestConstants.ValidEmail,
        AddressLine1 = "123 Test St",
        City = "Mumbai",
        State = "Maharashtra",
        PostalCode = "400001",
        Status = _status,
        IsAcceptingOrders = true,
        CommissionRate = 15.00m,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
    };
}
