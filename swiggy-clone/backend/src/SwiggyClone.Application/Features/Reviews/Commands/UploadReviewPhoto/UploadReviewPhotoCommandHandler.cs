using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Commands.UploadReviewPhoto;

internal sealed class UploadReviewPhotoCommandHandler(IFileStorageService fileStorageService)
    : IRequestHandler<UploadReviewPhotoCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UploadReviewPhotoCommand request, CancellationToken ct)
    {
        var url = await fileStorageService.UploadAsync(request.FileStream, request.FileName, "reviews", ct);
        return Result<string>.Success(url);
    }
}
