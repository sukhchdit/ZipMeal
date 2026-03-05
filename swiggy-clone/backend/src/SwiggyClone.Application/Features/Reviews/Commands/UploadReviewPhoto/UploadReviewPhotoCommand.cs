using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Commands.UploadReviewPhoto;

public sealed record UploadReviewPhotoCommand(
    Guid UserId,
    Stream FileStream,
    string FileName) : IRequest<Result<string>>;
