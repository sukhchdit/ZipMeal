namespace SwiggyClone.Application.Features.ChatSupport.DTOs;

public sealed record CannedResponseDto(
    Guid Id,
    string Title,
    string Content,
    int Category,
    int SortOrder);
