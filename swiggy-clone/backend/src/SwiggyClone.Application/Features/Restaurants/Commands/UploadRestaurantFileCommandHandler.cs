using MediatR;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

internal sealed class UploadRestaurantFileCommandHandler(
    IAppDbContext db,
    IFileStorageService fileStorageService)
    : IRequestHandler<UploadRestaurantFileCommand, Result<FileUploadResultDto>>
{
    private static readonly HashSet<string> ValidFileTypes = ["logo", "banner", "fssai", "gst"];

    public async Task<Result<FileUploadResultDto>> Handle(
        UploadRestaurantFileCommand request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (ownershipResult.IsFailure)
            return Result<FileUploadResultDto>.Failure(ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var fileType = request.FileType.ToLowerInvariant();

        if (!ValidFileTypes.Contains(fileType))
            return Result<FileUploadResultDto>.Failure("INVALID_FILE_TYPE", "File type must be one of: logo, banner, fssai, gst.");

        var folder = $"restaurants/{request.RestaurantId}/{fileType}";
        var url = await fileStorageService.UploadAsync(request.FileStream, request.FileName, folder, ct);

        var restaurant = ownershipResult.Value;

        switch (fileType)
        {
            case "logo":
                restaurant.LogoUrl = url;
                break;
            case "banner":
                restaurant.BannerUrl = url;
                break;
            case "fssai":
                restaurant.FssaiLicense = url;
                break;
            case "gst":
                restaurant.GstNumber = url;
                break;
        }

        restaurant.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);

        return Result<FileUploadResultDto>.Success(new FileUploadResultDto(url));
    }
}
