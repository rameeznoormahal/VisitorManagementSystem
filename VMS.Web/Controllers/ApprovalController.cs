using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VMS.Domain.Enums;
using VMS.Infrastructure.Data;
using VMS.Infrastructure.Identity;
using VMS.Web.Authorization;
using VMS.Web.ViewModels.Approvals;

namespace VMS.Web.Controllers;

public class ApprovalController : Controller
{
    private readonly VmsDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ApprovalController(
        VmsDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [RequirePermission("Visitor.Approve")]
    public async Task<IActionResult> Index()
    {
        var visits = await _context.VisitRequests
             .Include(x => x.VisitVisitors)
                 .ThenInclude(x => x.Visitor)
             .Include(x => x.Department)
             .Where(x => x.Status == VisitStatus.PendingApproval)
             .OrderBy(x => x.VisitFromDateTime)
             .ToListAsync();

        return View(visits);
    }

    [RequirePermission("Visitor.Approve")]
    [HttpGet]
    public async Task<IActionResult> Review(long id)
    {
        var visit = await _context.VisitRequests
             .Include(x => x.VisitVisitors)
                 .ThenInclude(x => x.Visitor)
             .Include(x => x.Department)
             .FirstOrDefaultAsync(x => x.VisitRequestId == id);

        if (visit == null)
            return NotFound();

        if (visit.Status != VisitStatus.PendingApproval)
        {
            TempData["ErrorMessage"] =
                "This visit request is no longer pending approval.";

            return RedirectToAction(nameof(Index));
        }

        var host = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == visit.HostUserId);

        var model = new ApprovalViewModel
        {
            VisitRequestId = visit.VisitRequestId,
            VisitReference = visit.VisitReference,

            HostName = host?.FullName ?? "Unknown",
            DepartmentName = visit.Department?.DepartmentName,

            VisitFromDateTime = visit.VisitFromDateTime,
            VisitToDateTime = visit.VisitToDateTime,

            Purpose = visit.Purpose,
            MeetingLocation = visit.MeetingLocation,
            Notes = visit.Notes,

            Visitors = visit.VisitVisitors
        .Select(x => new ApprovalVisitorViewModel
        {
            VisitorId = x.VisitorId,
            IdNumber = x.Visitor.IdNumber,
            IdType = x.Visitor.IdType,
            IdExpiryDate = x.Visitor.IdExpiryDate,
            FullName = x.Visitor.FullName,
            PhoneNumber = x.Visitor.PhoneNumber,
            Email = x.Visitor.Email,
            CompanyName = x.Visitor.CompanyName,
            Designation = x.Visitor.Designation,
            Nationality = x.Visitor.Nationality
        })
        .ToList()
        };

        return View(model);
    }

    [RequirePermission("Visitor.Approve")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(
        long id,
        string? comments)
    {
        var visit = await _context.VisitRequests
            .FirstOrDefaultAsync(x => x.VisitRequestId == id);

        if (visit == null)
            return NotFound();

        if (visit.Status != VisitStatus.PendingApproval)
        {
            TempData["ErrorMessage"] =
                "This request has already been processed.";

            return RedirectToAction(nameof(Index));
        }

        var currentUser =
            await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Unauthorized();

        visit.Status = VisitStatus.Approved;
        visit.DecisionByUserId = currentUser.Id;
        visit.DecisionDate = DateTime.UtcNow;
        visit.DecisionComments = comments?.Trim();
        visit.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            $"Visit request {visit.VisitReference} approved successfully.";

        return RedirectToAction(nameof(Index));
    }

    [RequirePermission("Visitor.Reject")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(
        long id,
        string? comments)
    {
        if (string.IsNullOrWhiteSpace(comments))
        {
            TempData["ErrorMessage"] =
                "Rejection comments are required.";

            return RedirectToAction(
                nameof(Review),
                new { id });
        }

        var visit = await _context.VisitRequests
            .FirstOrDefaultAsync(x => x.VisitRequestId == id);

        if (visit == null)
            return NotFound();

        if (visit.Status != VisitStatus.PendingApproval)
        {
            TempData["ErrorMessage"] =
                "This request has already been processed.";

            return RedirectToAction(nameof(Index));
        }

        var currentUser =
            await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Unauthorized();

        visit.Status = VisitStatus.Rejected;
        visit.DecisionByUserId = currentUser.Id;
        visit.DecisionDate = DateTime.UtcNow;
        visit.DecisionComments = comments.Trim();
        visit.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            $"Visit request {visit.VisitReference} rejected.";

        return RedirectToAction(nameof(Index));
    }
}