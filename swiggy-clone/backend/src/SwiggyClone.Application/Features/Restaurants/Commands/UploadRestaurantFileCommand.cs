using MediatR;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

public sealed record UploadRestaurantFileCommand(
    Guid RestaurantId,
    Guid OwnerId,
    string FileType,
    Stream FileStream,
    string FileName) : IRequest<Result<FileUploadResultDto>>;
