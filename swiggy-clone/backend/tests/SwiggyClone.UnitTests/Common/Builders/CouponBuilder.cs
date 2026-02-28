using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.UnitTests.Common.Builders;

public sealed class CouponBuilder
{
    private Guid _id = TestConstants.CouponId;
    private string _code = TestConstants.ValidCouponCode;
    private DiscountType _discountType = DiscountType.Percentage;
    private int _discountValue = 20;
    private int _minOrderAmount = 20000;
    private bool _isActive = true;

    public CouponBuilder WithId(Guid id) { _id = id; return this; }
    public CouponBuilder WithCode(string code) { _code = code; return this; }
    public CouponBuilder WithDiscountType(DiscountType type) { _discountType = type; return this; }
    public CouponBuilder WithDiscountValue(int value) { _discountValue = value; return this; }
    public CouponBuilder WithMinOrderAmount(int amount) { _minOrderAmount = amount; return this; }
    public CouponBuilder WithIsActive(bool active) { _isActive = active; return this; }

    public Coupon Build() => new()
    {
        Id = _id,
        Code = _code,
        Title = $"Save {_discountValue}%",
        DiscountType = _discountType,
        DiscountValue = _discountValue,
        MinOrderAmount = _minOrderAmount,
        ValidFrom = DateTimeOffset.UtcNow.AddDays(-1),
        ValidUntil = DateTimeOffset.UtcNow.AddDays(30),
        MaxUsagesPerUser = 1,
        IsActive = _isActive,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
    };
}
