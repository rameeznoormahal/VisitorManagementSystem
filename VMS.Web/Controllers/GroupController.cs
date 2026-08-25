using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VMS.Domain.Entities;
using VMS.Infrastructure.Data;
using VMS.Web.Authorization;
using VMS.Web.ViewModels.Groups;

namespace VMS.Web.Controllers;

[RequirePermission("Group.Manage")]
public class GroupController : Controller
{
    private readonly VmsDbContext _context;

    public GroupController(VmsDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var groups = await _context.Groups
            .AsNoTracking()
            .OrderBy(x => x.GroupName)
            .ToListAsync();

        return View(groups);
    }
    [HttpGet]
    public async Task<IActionResult> Index(string? search,string? status)
    {
        var query = _context.Groups
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.GroupName.Contains(search));
        }

        switch (status?.ToLower())
        {
            case "active":
                query = query.Where(x => x.IsActive);
                break;

            case "inactive":
                query = query.Where(x => !x.IsActive);
                break;
        }

        var groups = await query
            .OrderBy(x => x.GroupName)
            .ToListAsync();

        ViewBag.Search = search;
        ViewBag.Status = status;

        ViewBag.TotalGroups =
            await _context.Groups.CountAsync();

        ViewBag.ActiveGroups =
            await _context.Groups.CountAsync(x =>
                x.IsActive);

        ViewBag.InactiveGroups =
            await _context.Groups.CountAsync(x =>
                !x.IsActive);

        return View(groups);
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new GroupViewModel
        {
            Permissions = await _context.Permissions
                .Where(x => x.IsActive)
                .OrderBy(x => x.PermissionName)
                .Select(x => new PermissionSelectionViewModel
                {
                    PermissionId = x.PermissionId,
                    PermissionCode = x.PermissionCode,
                    PermissionName = x.PermissionName
                })
                .ToListAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GroupViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await ReloadPermissions(model);
            return View(model);
        }

        var groupName = model.GroupName.Trim();

        var exists = await _context.Groups
            .AnyAsync(x => x.GroupName == groupName);

        if (exists)
        {
            ModelState.AddModelError(
                nameof(model.GroupName),
                "Group name already exists.");

            await ReloadPermissions(model);

            return View(model);
        }

        var group = new Group
        {
            GroupName = groupName,
            Description = model.Description?.Trim(),
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        _context.Groups.Add(group);
        await _context.SaveChangesAsync();

        var selectedPermissionIds = model.Permissions
            .Where(x => x.IsSelected)
            .Select(x => x.PermissionId)
            .Distinct()
            .ToList();

        var validPermissionIds = await _context.Permissions
            .Where(x =>
                x.IsActive &&
                selectedPermissionIds.Contains(x.PermissionId))
            .Select(x => x.PermissionId)
            .ToListAsync();

        foreach (var permissionId in validPermissionIds)
        {
            _context.GroupPermissions.Add(new GroupPermission
            {
                GroupId = group.GroupId,
                PermissionId = permissionId
            });
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var group = await _context.Groups
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.GroupId == id);

        if (group == null)
            return NotFound();

        if (group.GroupName == "Administrators")
        {
            TempData["ErrorMessage"] =
                "The Administrators group is protected and cannot be edited.";

            return RedirectToAction(nameof(Index));
        }
       

        var assignedPermissionIds = await _context.GroupPermissions
            .Where(x => x.GroupId == id)
            .Select(x => x.PermissionId)
            .ToListAsync();

        var model = new GroupViewModel
        {
            GroupId = group.GroupId,
            GroupName = group.GroupName,
            Description = group.Description,
            IsActive = group.IsActive,

            Permissions = await _context.Permissions
                .Where(x => x.IsActive)
                .OrderBy(x => x.PermissionName)
                .Select(x => new PermissionSelectionViewModel
                {
                    PermissionId = x.PermissionId,
                    PermissionCode = x.PermissionCode,
                    PermissionName = x.PermissionName,
                    IsSelected =
                        assignedPermissionIds.Contains(x.PermissionId)
                })
                .ToListAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(GroupViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await ReloadPermissions(model);
            return View(model);
        }

        var group = await _context.Groups
            .FirstOrDefaultAsync(x => x.GroupId == model.GroupId);

        if (group == null)
            return NotFound();

        if (group.GroupName == "Administrators")
        {
            TempData["ErrorMessage"] =
                "The Administrators group is protected and cannot be deactivated.";

            return RedirectToAction(nameof(Index));
        }

        var groupName = model.GroupName.Trim();

        var duplicateName = await _context.Groups
            .AnyAsync(x =>
                x.GroupName == groupName &&
                x.GroupId != model.GroupId);

        if (duplicateName)
        {
            ModelState.AddModelError(
                nameof(model.GroupName),
                "Group name already exists.");

            await ReloadPermissions(model);

            return View(model);
        }

        group.GroupName = groupName;
        group.Description = model.Description?.Trim();
        group.IsActive = model.IsActive;
        group.UpdatedDate = DateTime.UtcNow;

        var existingPermissions = await _context.GroupPermissions
            .Where(x => x.GroupId == group.GroupId)
            .ToListAsync();

        _context.GroupPermissions.RemoveRange(existingPermissions);

        var selectedPermissionIds = model.Permissions
            .Where(x => x.IsSelected)
            .Select(x => x.PermissionId)
            .Distinct()
            .ToList();

        var validPermissionIds = await _context.Permissions
            .Where(x =>
                x.IsActive &&
                selectedPermissionIds.Contains(x.PermissionId))
            .Select(x => x.PermissionId)
            .ToListAsync();

        foreach (var permissionId in validPermissionIds)
        {
            _context.GroupPermissions.Add(new GroupPermission
            {
                GroupId = group.GroupId,
                PermissionId = permissionId
            });
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var group = await _context.Groups
            .FirstOrDefaultAsync(x => x.GroupId == id);

        if (group == null)
            return NotFound();

        group.IsActive = !group.IsActive;
        group.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private async Task ReloadPermissions(GroupViewModel model)
    {
        var selectedIds = model.Permissions
            .Where(x => x.IsSelected)
            .Select(x => x.PermissionId)
            .ToHashSet();

        model.Permissions = await _context.Permissions
            .Where(x => x.IsActive)
            .OrderBy(x => x.PermissionName)
            .Select(x => new PermissionSelectionViewModel
            {
                PermissionId = x.PermissionId,
                PermissionCode = x.PermissionCode,
                PermissionName = x.PermissionName,
                IsSelected = selectedIds.Contains(x.PermissionId)
            })
            .ToListAsync();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(long id)
    {
        var group = await _context.Groups
            .FirstOrDefaultAsync(x => x.GroupId == id);

        if (group == null)
            return NotFound();

        if (group.GroupName == "Administrators")
        {
            TempData["ErrorMessage"] =
                "The Administrators group is protected and cannot be deactivated.";

            return RedirectToAction(nameof(Index));
        }

        if (!group.IsActive)
        {
            TempData["ErrorMessage"] =
                "This group is already inactive.";

            return RedirectToAction(nameof(Index));
        }

        group.IsActive = false;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            $"Group '{group.GroupName}' deactivated successfully.";

        return RedirectToAction(nameof(Index));
    }
}