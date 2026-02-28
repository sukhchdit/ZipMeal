using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Admin.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Admin.Queries;

internal sealed class GetAdminUsersQueryHandler(IAppDbContext db)
    : IRequestHandler<GetAdminUsersQuery, Result<PagedResult<AdminUserDto>>>
{
    public async Task<Result<PagedResult<AdminUserDto>>> Handle(
        GetAdminUsersQuery request, CancellationToken ct)
    {
        var query = db.Users.AsNoTracking().AsQueryable();

        if (request.RoleFilter.HasValue)
            query = query.Where(u => u.Role == request.RoleFilter.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var pattern = $"%{request.Search.Trim()}%";
            query = query.Where(u =>
                EF.Functions.Like(u.FullName, pattern) ||
                EF.Functions.Like(u.PhoneNumber, pattern) ||
                (u.Email != null && EF.Functions.Like(u.Email, pattern)));
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new AdminUserDto(
                u.Id,
                u.FullName,
                u.PhoneNumber,
                u.Email,
                u.AvatarUrl,
                u.Role,
                u.IsVerified,
                u.IsActive,
                u.LastLoginAt,
                u.CreatedAt))
            .ToListAsync(ct);

        return Result<PagedResult<AdminUserDto>>.Success(
            new PagedResult<AdminUserDto>(items, totalCount, request.Page, request.PageSize));
    }
}
