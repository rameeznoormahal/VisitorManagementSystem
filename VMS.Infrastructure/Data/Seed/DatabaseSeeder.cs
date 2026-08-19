using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VMS.Domain.Entities;
using VMS.Infrastructure.Identity;

namespace VMS.Infrastructure.Data.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        VmsDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        // 1. Ensure database and migrations are ready
        await context.Database.MigrateAsync();

        // 2. Seed permissions
        var permissions = new[]
        {
            new Permission
            {
                PermissionCode = "Visitor.Create",
                PermissionName = "Create Visitor"
            },
            new Permission
            {
                PermissionCode = "Visitor.View",
                PermissionName = "View Visitor"
            },
            new Permission
            {
                PermissionCode = "Visitor.Edit",
                PermissionName = "Edit Visitor"
            },
            new Permission
            {
                PermissionCode = "Visitor.Cancel",
                PermissionName = "Cancel Visitor"
            },
            new Permission
            {
                PermissionCode = "Visitor.Approve",
                PermissionName = "Approve Visitor"
            },
            new Permission
            {
                PermissionCode = "Visitor.Reject",
                PermissionName = "Reject Visitor"
            },
            new Permission
            {
                PermissionCode = "Visitor.ValidateQR",
                PermissionName = "Validate QR Code"
            },
            new Permission
            {
                PermissionCode = "Visitor.CheckIn",
                PermissionName = "Check In Visitor"
            },
            new Permission
            {
                PermissionCode = "Visitor.CheckOut",
                PermissionName = "Check Out Visitor"
            },
            new Permission
            {
                PermissionCode = "Report.View",
                PermissionName = "View Reports"
            },
            new Permission
            {
                PermissionCode = "Report.Export",
                PermissionName = "Export Reports"
            },
            new Permission
            {
                PermissionCode = "User.Manage",
                PermissionName = "Manage Users"
            },
            new Permission
            {
                PermissionCode = "Group.Manage",
                PermissionName = "Manage Groups"
            },
            new Permission
            {
                PermissionCode = "Department.Manage",
                PermissionName = "Manage Departments"
            }
        };

        foreach (var permission in permissions)
        {
            var exists = await context.Permissions
                .AnyAsync(x =>
                    x.PermissionCode == permission.PermissionCode);

            if (!exists)
            {
                context.Permissions.Add(permission);
            }
        }

        await context.SaveChangesAsync();

        // 3. Create Administrators group
        var adminGroup = await context.Groups
            .FirstOrDefaultAsync(x =>
                x.GroupName == "Administrators");

        if (adminGroup == null)
        {
            adminGroup = new Group
            {
                GroupName = "Administrators",
                Description = "Full system administration access",
                IsActive = true
            };

            context.Groups.Add(adminGroup);

            await context.SaveChangesAsync();
        }

        // 4. Give Administrators group all permissions
        var allPermissions =
            await context.Permissions.ToListAsync();

        foreach (var permission in allPermissions)
        {
            var exists = await context.GroupPermissions
                .AnyAsync(x =>
                    x.GroupId == adminGroup.GroupId &&
                    x.PermissionId == permission.PermissionId);

            if (!exists)
            {
                context.GroupPermissions.Add(
                    new GroupPermission
                    {
                        GroupId = adminGroup.GroupId,
                        PermissionId = permission.PermissionId
                    });
            }
        }

        await context.SaveChangesAsync();

        // 5. Create default administrator user
        const string adminEmail = "admin@vms.local";
        const string adminPassword = "Admin@12345";

        var adminUser =
            await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,

                EmployeeCode = "ADMIN001",
                FullName = "System Administrator",
                JobTitle = "Administrator",

                IsActive = true
            };

            var result = await userManager.CreateAsync(
                adminUser,
                adminPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(x => x.Description));

                throw new Exception(
                    $"Unable to create default administrator: {errors}");
            }
        }

        // 6. Assign admin user to Administrators group
        var adminUserGroupExists =
            await context.UserGroups.AnyAsync(x =>
                x.UserId == adminUser.Id &&
                x.GroupId == adminGroup.GroupId);

        if (!adminUserGroupExists)
        {
            context.UserGroups.Add(
                new UserGroup
                {
                    UserId = adminUser.Id,
                    GroupId = adminGroup.GroupId
                });

            await context.SaveChangesAsync();
        }
    }
}