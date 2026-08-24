using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VMS.Domain.Entities;
using VMS.Infrastructure.Data;
using VMS.Web.Authorization;

namespace VMS.Web.Controllers;

[RequirePermission("Department.Manage")]
public class DepartmentController : Controller
{
    private readonly VmsDbContext _context;

    public DepartmentController(VmsDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var departments = await _context.Departments
            .OrderBy(x => x.DepartmentName)
            .ToListAsync();

        return View(departments);
    }
    [HttpGet]
    public async Task<IActionResult> Index(string? search, string? status)
    {
        var query = _context.Departments
            .AsNoTracking()
            .AsQueryable();

        // Search by department code or name
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.DepartmentCode.Contains(search) ||
                x.DepartmentName.Contains(search));
        }

        // Status filter
        switch (status?.ToLower())
        {
            case "active":
                query = query.Where(x => x.IsActive);
                break;

            case "inactive":
                query = query.Where(x => !x.IsActive);
                break;
        }

        var departments = await query
            .OrderBy(x => x.DepartmentName)
            .ToListAsync();

        ViewBag.Search = search;
        ViewBag.Status = status;

        // Summary cards should represent ALL departments,
        // not only the filtered result.
        ViewBag.TotalDepartments =
            await _context.Departments.CountAsync();

        ViewBag.ActiveDepartments =
            await _context.Departments.CountAsync(x =>
                x.IsActive);

        ViewBag.InactiveDepartments =
            await _context.Departments.CountAsync(x =>
                !x.IsActive);

        return View(departments);
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Department model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var codeExists = await _context.Departments
            .AnyAsync(x => x.DepartmentCode == model.DepartmentCode);

        if (codeExists)
        {
            ModelState.AddModelError(
                nameof(model.DepartmentCode),
                "Department code already exists.");

            return View(model);
        }

        var nameExists = await _context.Departments
            .AnyAsync(x => x.DepartmentName == model.DepartmentName);

        if (nameExists)
        {
            ModelState.AddModelError(
                nameof(model.DepartmentName),
                "Department name already exists.");

            return View(model);
        }

        model.CreatedDate = DateTime.UtcNow;
        model.IsActive = true;

        _context.Departments.Add(model);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var department = await _context.Departments
            .FindAsync(id);

        if (department == null)
            return NotFound();

        return View(department);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Department model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var department = await _context.Departments
            .FindAsync(model.DepartmentId);

        if (department == null)
            return NotFound();

        var duplicateCode = await _context.Departments
            .AnyAsync(x =>
                x.DepartmentCode == model.DepartmentCode &&
                x.DepartmentId != model.DepartmentId);

        if (duplicateCode)
        {
            ModelState.AddModelError(
                nameof(model.DepartmentCode),
                "Department code already exists.");

            return View(model);
        }

        department.DepartmentCode = model.DepartmentCode;
        department.DepartmentName = model.DepartmentName;
        department.Description = model.Description;
        department.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var department = await _context.Departments
            .FindAsync(id);

        if (department == null)
            return NotFound();

        department.IsActive = !department.IsActive;
        department.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}