using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VMS.Infrastructure.Data;
using VMS.Infrastructure.Identity;

namespace VMS.Web.Services;

public class PermissionService : IPermissionService
{
    private readonly VmsDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PermissionService(
        VmsDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<bool> HasPermissionAsync(
        ClaimsPrincipal principal,
        string permissionCode)
    {
        if (principal.Identity?.IsAuthenticated != true)
            return false;

        var user = await _userManager.GetUserAsync(principal);

        if (user == null || !user.IsActive)
            return false;

        return await (
            from userGroup in _context.UserGroups

            join grp in _context.Groups
                on userGroup.GroupId equals grp.GroupId

            join groupPermission in _context.GroupPermissions
                on grp.GroupId equals groupPermission.GroupId

            join permission in _context.Permissions
                on groupPermission.PermissionId equals permission.PermissionId

            where userGroup.UserId == user.Id
                  && grp.IsActive
                  && permission.IsActive
                  && permission.PermissionCode == permissionCode

            select permission.PermissionId
        ).AnyAsync();
    }
}