namespace SwiggyClone.Application.Features.DineIn.DTOs;

public sealed record DineInTableDto(
    Guid Id,
    string TableNumber,
    int Capacity,
    string? FloorSection);
