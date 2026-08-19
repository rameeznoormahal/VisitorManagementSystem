using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VMS.Domain.Entities;
using VMS.Infrastructure.Data;
using VMS.Infrastructure.Identity;
using VMS.Web.Authorization;
using VMS.Web.ViewModels.Users;

namespace VMS.Web.Controllers;

[RequirePermission("User.Manage")]
public class UserController : Controller
{
    private readonly VmsDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(
        VmsDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _context.Users
            .Include(x => x.Department)
            .OrderBy(x => x.FullName)
            .ToListAsync();

        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new UserCreateViewModel();

        await LoadLookups(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadLookups(model);
            return View(model);
        }

        var emailExists = await _userManager
            .FindByEmailAsync(model.Email);

        if (emailExists != null)
        {
            ModelState.AddModelError(
                nameof(model.Email),
                "A user with this email already exists.");

            await LoadLookups(model);
            return View(model);
        }

        var employeeCodeExists = await _context.Users
            .AnyAsync(x => x.EmployeeCode == model.EmployeeCode);

        if (employeeCodeExists)
        {
            ModelState.AddModelError(
                nameof(model.EmployeeCode),
                "Employee code already exists.");

            await LoadLookups(model);
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email.Trim(),
            Email = model.Email.Trim(),
            EmailConfirmed = true,

            EmployeeCode = model.EmployeeCode.Trim(),
            FullName = model.FullName.Trim(),
            PhoneNumber = model.PhoneNumber?.Trim(),
            JobTitle = model.JobTitle?.Trim(),

            DepartmentId = model.DepartmentId,
            ManagerUserId = model.ManagerUserId,

            IsActive = model.IsActive,
            CreatedDate = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(
            user,
            model.TemporaryPassword);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            await LoadLookups(model);
            return View(model);
        }

        var validGroupIds = await _context.Groups
            .Where(x =>
                x.IsActive &&
                model.SelectedGroupIds.Contains(x.GroupId))
            .Select(x => x.GroupId)
            .ToListAsync();

        foreach (var groupId in validGroupIds)
        {
            _context.UserGroups.Add(new UserGroup
            {
                UserId = user.Id,
                GroupId = groupId
            });
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadLookups(
        UserCreateViewModel model)
    {
        model.Departments = await _context.Departments
            .Where(x => x.IsActive)
            .OrderBy(x => x.DepartmentName)
            .Select(x => new SelectListItem
            {
                Value = x.DepartmentId.ToString(),
                Text = x.DepartmentName
            })
            .ToListAsync();

        model.Managers = await _context.Users
            .Where(x => x.IsActive)
            .OrderBy(x => x.FullName)
            .Select(x => new SelectListItem
            {
                Value = x.Id,
                Text = x.FullName
            })
            .ToListAsync();

        model.Groups = await _context.Groups
            .Where(x => x.IsActive)
            .OrderBy(x => x.GroupName)
            .Select(x => new SelectListItem
            {
                Value = x.GroupId.ToString(),
                Text = x.GroupName
            })
            .ToListAsync();
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
            return NotFound();

        var groupIds = await _context.UserGroups
            .Where(x => x.UserId == id)
            .Select(x => x.GroupId)
            .ToListAsync();

        var model = new UserEditViewModel
        {
            UserId = user.Id,
            EmployeeCode = user.EmployeeCode,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            JobTitle = user.JobTitle,
            DepartmentId = user.DepartmentId,
            ManagerUserId = user.ManagerUserId,
            IsActive = user.IsActive,
            SelectedGroupIds = groupIds
        };

        await LoadEditLookups(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadEditLookups(model);
            return View(model);
        }

        var user = await _userManager.FindByIdAsync(model.UserId);

        if (user == null)
            return NotFound();

        var employeeCodeExists = await _context.Users
            .AnyAsync(x =>
                x.EmployeeCode == model.EmployeeCode &&
                x.Id != model.UserId);

        if (employeeCodeExists)
        {
            ModelState.AddModelError(
                nameof(model.EmployeeCode),
                "Employee code already exists.");

            await LoadEditLookups(model);
            return View(model);
        }

        var emailUser = await _userManager.FindByEmailAsync(model.Email);

        if (emailUser != null && emailUser.Id != model.UserId)
        {
            ModelState.AddModelError(
                nameof(model.Email),
                "Email address is already used by another user.");

            await LoadEditLookups(model);
            return View(model);
        }

        // Prevent user from being their own manager
        if (model.ManagerUserId == model.UserId)
        {
            ModelState.AddModelError(
                nameof(model.ManagerUserId),
                "A user cannot be their own reporting manager.");

            await LoadEditLookups(model);
            return View(model);
        }

        user.EmployeeCode = model.EmployeeCode.Trim();
        user.FullName = model.FullName.Trim();

        user.Email = model.Email.Trim();
        user.UserName = model.Email.Trim();

        user.PhoneNumber = model.PhoneNumber?.Trim();
        user.JobTitle = model.JobTitle?.Trim();

        user.DepartmentId = model.DepartmentId;
        user.ManagerUserId = model.ManagerUserId;

        user.IsActive = model.IsActive;
        user.UpdatedDate = DateTime.UtcNow;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            await LoadEditLookups(model);
            return View(model);
        }

        // Remove existing group assignments
        var existingUserGroups = await _context.UserGroups
            .Where(x => x.UserId == user.Id)
            .ToListAsync();

        _context.UserGroups.RemoveRange(existingUserGroups);

        // Add new group assignments
        var validGroupIds = await _context.Groups
            .Where(x =>
                x.IsActive &&
                model.SelectedGroupIds.Contains(x.GroupId))
            .Select(x => x.GroupId)
            .ToListAsync();

        foreach (var groupId in validGroupIds)
        {
            _context.UserGroups.Add(new UserGroup
            {
                UserId = user.Id,
                GroupId = groupId
            });
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadEditLookups(UserEditViewModel model)
    {
        model.Departments = await _context.Departments
            .Where(x => x.IsActive)
            .OrderBy(x => x.DepartmentName)
            .Select(x => new SelectListItem
            {
                Value = x.DepartmentId.ToString(),
                Text = x.DepartmentName
            })
            .ToListAsync();

        model.Managers = await _context.Users
            .Where(x =>
                x.IsActive &&
                x.Id != model.UserId)
            .OrderBy(x => x.FullName)
            .Select(x => new SelectListItem
            {
                Value = x.Id,
                Text = x.FullName
            })
            .ToListAsync();

        model.Groups = await _context.Groups
            .Where(x => x.IsActive)
            .OrderBy(x => x.GroupName)
            .Select(x => new SelectListItem
            {
                Value = x.GroupId.ToString(),
                Text = x.GroupName
            })
            .ToListAsync();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound();

        user.IsActive = !user.IsActive;
        user.UpdatedDate = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> ResetPassword(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound();

        var model = new ResetPasswordViewModel
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(
    ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByIdAsync(model.UserId);

        if (user == null)
            return NotFound();

        var token =
            await _userManager.GeneratePasswordResetTokenAsync(user);

        var result =
            await _userManager.ResetPasswordAsync(
                user,
                token,
                model.NewPassword);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            return View(model);
        }

        TempData["SuccessMessage"] =
            $"Password for {user.FullName} has been reset successfully.";

        return RedirectToAction(nameof(Index));
    }
}