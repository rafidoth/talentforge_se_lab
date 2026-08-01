using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Dto;
using server.Entities;
using server.ServiceResults;
using server.Utils;

namespace server.Services.UserServices;

public class UserService(
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext db
) : IUserService
{
    public async Task<PagedResponse<UserDto>> GetUsersAsync(UserQueryFilter filter)
    {
        var query = userManager.Users.AsNoTracking();
        query = ApplySearchFilter(query, filter.Search!);
        query = ApplySortEmailFilter(query, filter.SortBy!);

        var mappedQuery = query.Select(u => new UserDto
        {
            Id = u.Id,
            Email = u.Email ?? string.Empty,
            JoinedAt = u.JoinedAt,
            Status = u.Status,
            LastLoginAt = u.LastLoginAt,
            Role = db.UserRoles
                .Where(ur => ur.UserId == u.Id)
                .Join(db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                .FirstOrDefault() ?? "Unknown",
            Method = db.UserLogins
                .Where(ul => ul.UserId == u.Id)
                .Select(ul => ul.LoginProvider)
                .FirstOrDefault() ?? "Email"
        });

        return await PagedResponse.CreateAsync(mappedQuery, filter.PageNumber, filter.PageSize, maxPageSize: 20);
    }

    private IQueryable<ApplicationUser> ApplySearchFilter(IQueryable<ApplicationUser> query, string search)
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u =>
                (u.UserName != null && u.UserName.ToLower().Contains(search)) ||
                (u.Email != null && u.Email.ToLower().Contains(search)));
        }
        return query;
    }

    private IQueryable<ApplicationUser> ApplySortEmailFilter(IQueryable<ApplicationUser> query, string sortBy)
    {
        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            if (sortBy.Equals("email desc", StringComparison.OrdinalIgnoreCase))
                query = query.OrderByDescending(u => u.Email);
            else query = query.OrderBy(u => u.Email);
        }
        return query;
    }

    public Task<ServiceResult<bool>> BlockUserAsync(List<string> userIds) => throw new NotImplementedException();
    public Task<ServiceResult<bool>> UnblockUserAsync(List<string> userIds) => throw new NotImplementedException();
    public Task<ServiceResult<bool>> AssignRoleToUserAsync(List<string> userIds, string roleName) => throw new NotImplementedException();
    public Task<ServiceResult<bool>> DeleteUserAsync(List<string> userIds) => throw new NotImplementedException();
}
