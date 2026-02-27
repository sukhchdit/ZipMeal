using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Auth.Commands;

internal sealed class ChangePasswordCommandHandler(IAppDbContext db, IPasswordHasher passwordHasher)
    : IRequestHandler<ChangePasswordCommand, Result>
{
    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, ct);
        if (user is null)
            return Result.Failure("USER_NOT_FOUND", "User not found.");

        if (user.PasswordHash is null)
            return Result.Failure("NO_PASSWORD_SET", "No password is set for this account. Phone-only accounts cannot change password.");

        if (!passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            return Result.Failure("INVALID_PASSWORD", "Current password is incorrect.");

        user.PasswordHash = passwordHasher.Hash(request.NewPassword);
        user.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
