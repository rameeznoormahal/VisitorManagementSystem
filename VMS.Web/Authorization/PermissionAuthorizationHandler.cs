using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VMS.Domain.Entities;
using VMS.Infrastructure.Data;
using VMS.Infrastructure.Identity;

namespace VMS.Web.Authorization;

public class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly VmsDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PermissionAuthorizationHandler(
        VmsDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
            return;

        var user = await _userManager.GetUserAsync(context.User);

        if (user == null || !user.IsActive)
            return;

        var hasPermission =
    await (
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
              && permission.PermissionCode == requirement.Permission

        select permission.PermissionId
    ).AnyAsync();
        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}