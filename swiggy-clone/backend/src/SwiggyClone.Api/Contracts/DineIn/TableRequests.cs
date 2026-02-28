using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Api.Contracts.DineIn;

public sealed record CreateTableRequest(
    string TableNumber,
    int Capacity = 4,
    string? FloorSection = null);

public sealed record UpdateTableRequest(
    string? TableNumber = null,
    int? Capacity = null,
    string? FloorSection = null,
    bool? IsActive = null,
    TableStatus? Status = null);

public sealed record UpdateDineInOrderStatusRequest(
    DineInOrderStatus NewStatus,
    string? Notes = null);
