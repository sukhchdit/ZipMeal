namespace SwiggyClone.UnitTests.Common;

public static class TestConstants
{
    public static readonly Guid UserId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
    public static readonly Guid OwnerId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901");
    public static readonly Guid RestaurantId = Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012");
    public static readonly Guid OrderId = Guid.Parse("d4e5f6a7-b8c9-0123-defa-234567890123");
    public static readonly Guid WalletId = Guid.Parse("e5f6a7b8-c9d0-1234-efab-345678901234");
    public static readonly Guid CouponId = Guid.Parse("f6a7b8c9-d0e1-2345-fabc-456789012345");
    public static readonly Guid AddressId = Guid.Parse("a7b8c9d0-e1f2-3456-abcd-567890123456");
    public static readonly Guid TransactionId = Guid.Parse("b8c9d0e1-f2a3-4567-bcde-678901234567");

    public const string ValidEmail = "test@example.com";
    public const string ValidPhone = "+919876543210";
    public const string ValidPassword = "P@ssw0rd!";
    public const string ValidFullName = "Test User";
    public const string ValidSlug = "test-restaurant";
    public const string ValidCouponCode = "SAVE20";
}
