using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VMS.Infrastructure.Data;
using VMS.Web.Authorization;

namespace VMS.Web.Controllers;

[Authorize]
public class VisitorController : Controller
{
    private readonly VmsDbContext _context;

    public VisitorController(VmsDbContext context)
    {
        _context = context;
    }

    [RequirePermission("Visitor.View")]
    [HttpGet]
    public async Task<IActionResult> Index(
        string? filter,
        string? search)
    {
        var query = _context.Visitors
            .AsNoTracking()
            .AsQueryable();

        var today = DateOnly.FromDateTime(DateTime.Today);

        switch (filter?.ToLower())
        {
            case "active":
                query = query.Where(x =>
                    x.IsActive &&
                    x.IdExpiryDate >= today);
                break;

            case "expired":
                query = query.Where(x =>
                    x.IdExpiryDate < today);
                break;
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.FullName.Contains(search) ||
                x.IdNumber.Contains(search) ||
                (x.CompanyName != null &&
                 x.CompanyName.Contains(search)) ||
                (x.PhoneNumber != null &&
                 x.PhoneNumber.Contains(search)));
        }

        var visitors = await query
            .OrderBy(x => x.FullName)
            .ToListAsync();

        ViewBag.Filter = filter;
        ViewBag.Search = search;

        return View(visitors);
    }

    [RequirePermission("Visitor.View")]
    [HttpGet]
    public async Task<IActionResult> Details(long id)
    {
        var visitor = await _context.Visitors
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.VisitorId == id);

        if (visitor == null)
            return NotFound();

        var visitHistory = await _context.VisitVisitors
            .Where(x => x.VisitorId == id)
            .Include(x => x.VisitRequest)
                .ThenInclude(x => x.Department)
            .OrderByDescending(x => x.VisitRequest.VisitFromDateTime)
            .ToListAsync();

        var accessHistory = await _context.VisitAccessLogs
            .Where(x => x.VisitVisitor.VisitorId == id)
            .Include(x => x.VisitRequest)
            .OrderByDescending(x => x.EntryTime)
            .ToListAsync();

        ViewBag.VisitHistory = visitHistory;
        ViewBag.AccessHistory = accessHistory;

        return View(visitor);
    }

    [RequirePermission("Visitor.Edit")]
    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        var visitor = await _context.Visitors
            .FirstOrDefaultAsync(x => x.VisitorId == id);

        if (visitor == null)
            return NotFound();

        return View(visitor);
    }

    [RequirePermission("Visitor.Edit")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(VMS.Domain.Entities.Visitor model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var visitor = await _context.Visitors
            .FirstOrDefaultAsync(x => x.VisitorId == model.VisitorId);

        if (visitor == null)
            return NotFound();

        var duplicateId = await _context.Visitors
            .AnyAsync(x =>
                x.IdNumber == model.IdNumber &&
                x.VisitorId != model.VisitorId);

        if (duplicateId)
        {
            ModelState.AddModelError(
                nameof(model.IdNumber),
                "Another visitor already uses this ID number.");

            return View(model);
        }

        visitor.IdType = model.IdType.Trim();
        visitor.IdNumber = model.IdNumber.Trim();
        visitor.IdExpiryDate = model.IdExpiryDate;

        visitor.FullName = model.FullName.Trim();
        visitor.PhoneNumber = model.PhoneNumber.Trim();

        visitor.Email = model.Email?.Trim();
        visitor.CompanyName = model.CompanyName?.Trim();
        visitor.Designation = model.Designation?.Trim();
        visitor.Nationality = model.Nationality?.Trim();

        visitor.IsActive = model.IsActive;
        visitor.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Visitor information updated successfully.";

        return RedirectToAction(
            nameof(Details),
            new { id = visitor.VisitorId });
    }
}