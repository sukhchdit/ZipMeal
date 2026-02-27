using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Banners.Commands;

public sealed record ToggleBannerCommand(Guid Id, bool IsActive) : IRequest<Result>;
