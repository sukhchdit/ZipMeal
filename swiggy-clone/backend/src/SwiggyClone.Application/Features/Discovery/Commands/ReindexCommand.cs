using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Discovery.Commands;

public sealed record ReindexCommand(bool Recreate = false) : IRequest<Result<ReindexResultDto>>;

public sealed record ReindexResultDto(
    int RestaurantsIndexed,
    int MenuItemsIndexed,
    long ElapsedMilliseconds);
