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
    public static readonly Guid ReviewId = Guid.Parse("c9d0e1f2-a3b4-5678-cdef-789012345678");
    public static readonly Guid GroupOrderId = Guid.Parse("d0e1f2a3-b4c5-6789-defa-890123456789");
    public static readonly Guid LoyaltyAccountId = Guid.Parse("e1f2a3b4-c5d6-7890-efab-901234567890");
    public static readonly Guid LoyaltyRewardId = Guid.Parse("f2a3b4c5-d6e7-8901-fabc-012345678901");
    public static readonly Guid DisputeId = Guid.Parse("a3b4c5d6-e7f8-9012-abcd-123456789abc");
    public static readonly Guid ExperimentId = Guid.Parse("b4c5d6e7-f8a9-0123-bcde-234567890bcd");
    public static readonly Guid MenuItemId = Guid.Parse("c5d6e7f8-a9b0-1234-cdef-345678901cde");
    public static readonly Guid CategoryId = Guid.Parse("d6e7f8a9-b0c1-2345-defa-456789012def");
    public static readonly Guid ParticipantUserId = Guid.Parse("e7f8a9b0-c1d2-3456-efab-567890123ef0");
    public static readonly Guid AdminId = Guid.Parse("f8a9b0c1-d2e3-4567-fabc-678901234f01");

    public const string ValidEmail = "test@example.com";
    public const string ValidPhone = "+919876543210";
    public const string ValidPassword = "P@ssw0rd!";
    public const string ValidFullName = "Test User";
    public const string ValidSlug = "test-restaurant";
    public const string ValidCouponCode = "SAVE20";
    public const string ValidInviteCode = "A3K9X2";
}
