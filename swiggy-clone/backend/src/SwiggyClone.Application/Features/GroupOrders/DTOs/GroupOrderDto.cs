using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Application.Features.GroupOrders.DTOs;

public sealed record GroupOrderDto(
    Guid Id,
    Guid RestaurantId,
    string RestaurantName,
    string? RestaurantLogoUrl,
    Guid InitiatorUserId,
    string InitiatorName,
    string InviteCode,
    GroupOrderStatus Status,
    PaymentSplitType PaymentSplitType,
    Guid? DeliveryAddressId,
    string? SpecialInstructions,
    DateTimeOffset ExpiresAt,
    DateTimeOffset? FinalizedAt,
    Guid? OrderId,
    DateTimeOffset CreatedAt,
    List<GroupOrderParticipantDto> Participants);

public sealed record GroupOrderParticipantDto(
    Guid Id,
    Guid UserId,
    string UserName,
    string? AvatarUrl,
    bool IsInitiator,
    GroupOrderParticipantStatus Status,
    DateTimeOffset JoinedAt,
    DateTimeOffset? LeftAt,
    int ItemCount,
    int ItemsTotal);
