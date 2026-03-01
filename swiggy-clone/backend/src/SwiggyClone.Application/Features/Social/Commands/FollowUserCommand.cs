using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Social.Notifications;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Social.Commands;

public sealed record FollowUserCommand(Guid FollowerId, Guid FollowingId) : IRequest<Result>;

internal sealed class FollowUserCommandValidator : AbstractValidator<FollowUserCommand>
{
    public FollowUserCommandValidator()
    {
        RuleFor(x => x.FollowerId).NotEmpty();
        RuleFor(x => x.FollowingId).NotEmpty();
        RuleFor(x => x)
            .Must(x => x.FollowerId != x.FollowingId)
            .WithMessage("CANNOT_FOLLOW_SELF")
            .WithErrorCode("CANNOT_FOLLOW_SELF");
    }
}

internal sealed class FollowUserCommandHandler(IAppDbContext db, IPublisher publisher)
    : IRequestHandler<FollowUserCommand, Result>
{
    public async Task<Result> Handle(FollowUserCommand request, CancellationToken ct)
    {
        if (request.FollowerId == request.FollowingId)
            return Result.Failure("CANNOT_FOLLOW_SELF", "You cannot follow yourself.");

        var targetExists = await db.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == request.FollowingId, ct);

        if (!targetExists)
            return Result.Failure("USER_NOT_FOUND", "User not found.");

        var alreadyFollowing = await db.UserFollows
            .AsNoTracking()
            .AnyAsync(f => f.FollowerId == request.FollowerId && f.FollowingId == request.FollowingId, ct);

        if (alreadyFollowing)
            return Result.Failure("ALREADY_FOLLOWING", "You are already following this user.");

        db.UserFollows.Add(new UserFollow
        {
            FollowerId = request.FollowerId,
            FollowingId = request.FollowingId,
            CreatedAt = DateTimeOffset.UtcNow,
        });

        await db.SaveChangesAsync(ct);

        await publisher.Publish(
            new UserFollowedNotification(request.FollowerId, request.FollowingId), ct);

        return Result.Success();
    }
}
