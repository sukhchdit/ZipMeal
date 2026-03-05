using FluentAssertions;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.UnitTests.Domain;

public sealed class GroupOrderTests
{
    [Fact]
    public void NewGroupOrder_InviteCode_DefaultsToEmpty()
    {
        var groupOrder = new GroupOrder();

        groupOrder.InviteCode.Should().BeEmpty();
    }

    [Fact]
    public void NewGroupOrder_Status_DefaultsToActive()
    {
        var groupOrder = new GroupOrder();

        groupOrder.Status.Should().Be(GroupOrderStatus.Active);
    }

    [Fact]
    public void NewGroupOrder_ParticipantsCollection_IsInitializedEmpty()
    {
        var groupOrder = new GroupOrder();

        groupOrder.Participants.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void NewGroupOrder_NullableProperties_AreNull()
    {
        var groupOrder = new GroupOrder();

        groupOrder.DeliveryAddressId.Should().BeNull();
        groupOrder.SpecialInstructions.Should().BeNull();
        groupOrder.FinalizedAt.Should().BeNull();
        groupOrder.OrderId.Should().BeNull();
        groupOrder.DeliveryAddress.Should().BeNull();
        groupOrder.Order.Should().BeNull();
    }

    [Fact]
    public void GroupOrder_SetProperties_RetainsValues()
    {
        var id = Guid.NewGuid();
        var restaurantId = Guid.NewGuid();
        var initiatorId = Guid.NewGuid();
        var addressId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddMinutes(60);
        var finalizedAt = now.AddMinutes(45);

        var groupOrder = new GroupOrder
        {
            Id = id,
            RestaurantId = restaurantId,
            InitiatorUserId = initiatorId,
            InviteCode = "A3K9X2",
            Status = GroupOrderStatus.OrderPlaced,
            PaymentSplitType = PaymentSplitType.SplitEqual,
            DeliveryAddressId = addressId,
            SpecialInstructions = "Ring the doorbell",
            ExpiresAt = expiresAt,
            FinalizedAt = finalizedAt,
            OrderId = orderId,
            CreatedAt = now,
            UpdatedAt = now
        };

        groupOrder.Id.Should().Be(id);
        groupOrder.RestaurantId.Should().Be(restaurantId);
        groupOrder.InitiatorUserId.Should().Be(initiatorId);
        groupOrder.InviteCode.Should().Be("A3K9X2");
        groupOrder.Status.Should().Be(GroupOrderStatus.OrderPlaced);
        groupOrder.PaymentSplitType.Should().Be(PaymentSplitType.SplitEqual);
        groupOrder.DeliveryAddressId.Should().Be(addressId);
        groupOrder.SpecialInstructions.Should().Be("Ring the doorbell");
        groupOrder.ExpiresAt.Should().Be(expiresAt);
        groupOrder.FinalizedAt.Should().Be(finalizedAt);
        groupOrder.OrderId.Should().Be(orderId);
    }

    [Fact]
    public void GroupOrder_ParticipantsCollection_CanAddParticipants()
    {
        var groupOrder = new GroupOrder();
        var participant = new GroupOrderParticipant
        {
            Id = Guid.NewGuid(),
            GroupOrderId = groupOrder.Id,
            UserId = Guid.NewGuid(),
            IsInitiator = true,
            Status = GroupOrderParticipantStatus.Joined,
            JoinedAt = DateTimeOffset.UtcNow
        };

        groupOrder.Participants.Add(participant);

        groupOrder.Participants.Should().HaveCount(1);
        groupOrder.Participants.Should().Contain(participant);
    }

    [Fact]
    public void GroupOrder_AllStatusValues_CanBeSet()
    {
        var groupOrder = new GroupOrder();

        groupOrder.Status = GroupOrderStatus.Active;
        groupOrder.Status.Should().Be(GroupOrderStatus.Active);

        groupOrder.Status = GroupOrderStatus.Finalizing;
        groupOrder.Status.Should().Be(GroupOrderStatus.Finalizing);

        groupOrder.Status = GroupOrderStatus.OrderPlaced;
        groupOrder.Status.Should().Be(GroupOrderStatus.OrderPlaced);

        groupOrder.Status = GroupOrderStatus.Expired;
        groupOrder.Status.Should().Be(GroupOrderStatus.Expired);

        groupOrder.Status = GroupOrderStatus.Cancelled;
        groupOrder.Status.Should().Be(GroupOrderStatus.Cancelled);
    }

    [Fact]
    public void GroupOrder_AllPaymentSplitTypes_CanBeSet()
    {
        var groupOrder = new GroupOrder();

        groupOrder.PaymentSplitType = PaymentSplitType.InitiatorPays;
        groupOrder.PaymentSplitType.Should().Be(PaymentSplitType.InitiatorPays);

        groupOrder.PaymentSplitType = PaymentSplitType.SplitEqual;
        groupOrder.PaymentSplitType.Should().Be(PaymentSplitType.SplitEqual);

        groupOrder.PaymentSplitType = PaymentSplitType.PayYourShare;
        groupOrder.PaymentSplitType.Should().Be(PaymentSplitType.PayYourShare);
    }

    [Fact]
    public void GroupOrder_ExpiresAt_TracksExpiration()
    {
        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddMinutes(60);

        var groupOrder = new GroupOrder
        {
            ExpiresAt = expiresAt
        };

        groupOrder.ExpiresAt.Should().Be(expiresAt);
        groupOrder.ExpiresAt.Should().BeAfter(now);
    }

    [Fact]
    public void GroupOrder_MultipleParticipants_CanBeAdded()
    {
        var groupOrder = new GroupOrder { Id = Guid.NewGuid() };
        var initiator = new GroupOrderParticipant
        {
            Id = Guid.NewGuid(),
            GroupOrderId = groupOrder.Id,
            UserId = Guid.NewGuid(),
            IsInitiator = true,
            Status = GroupOrderParticipantStatus.Joined
        };
        var joiner = new GroupOrderParticipant
        {
            Id = Guid.NewGuid(),
            GroupOrderId = groupOrder.Id,
            UserId = Guid.NewGuid(),
            IsInitiator = false,
            Status = GroupOrderParticipantStatus.Joined
        };

        groupOrder.Participants.Add(initiator);
        groupOrder.Participants.Add(joiner);

        groupOrder.Participants.Should().HaveCount(2);
        groupOrder.Participants.Should().ContainSingle(p => p.IsInitiator);
        groupOrder.Participants.Should().ContainSingle(p => !p.IsInitiator);
    }
}
