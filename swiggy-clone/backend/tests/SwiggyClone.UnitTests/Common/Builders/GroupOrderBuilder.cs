using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.UnitTests.Common.Builders;

public sealed class GroupOrderBuilder
{
    private Guid _id = TestConstants.GroupOrderId;
    private Guid _restaurantId = TestConstants.RestaurantId;
    private Guid _initiatorUserId = TestConstants.UserId;
    private string _inviteCode = TestConstants.ValidInviteCode;
    private GroupOrderStatus _status = GroupOrderStatus.Active;
    private PaymentSplitType _paymentSplitType = PaymentSplitType.SplitEqual;
    private DateTimeOffset _expiresAt = DateTimeOffset.UtcNow.AddMinutes(60);

    public GroupOrderBuilder WithId(Guid id) { _id = id; return this; }
    public GroupOrderBuilder WithRestaurantId(Guid restaurantId) { _restaurantId = restaurantId; return this; }
    public GroupOrderBuilder WithInitiatorUserId(Guid userId) { _initiatorUserId = userId; return this; }
    public GroupOrderBuilder WithInviteCode(string code) { _inviteCode = code; return this; }
    public GroupOrderBuilder WithStatus(GroupOrderStatus status) { _status = status; return this; }
    public GroupOrderBuilder WithPaymentSplitType(PaymentSplitType type) { _paymentSplitType = type; return this; }
    public GroupOrderBuilder WithExpiresAt(DateTimeOffset expiresAt) { _expiresAt = expiresAt; return this; }

    public GroupOrder Build() => new()
    {
        Id = _id,
        RestaurantId = _restaurantId,
        InitiatorUserId = _initiatorUserId,
        InviteCode = _inviteCode,
        Status = _status,
        PaymentSplitType = _paymentSplitType,
        ExpiresAt = _expiresAt,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
    };
}
