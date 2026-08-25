using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VMS.Application.Interfaces;
using VMS.Domain.Entities;
using VMS.Domain.Enums;
using VMS.Infrastructure.Data;
using VMS.Infrastructure.Identity;
using VMS.Web.Authorization;
using VMS.Web.ViewModels.Reception;

namespace VMS.Web.Controllers;

public class ReceptionController : Controller
{
    private readonly VmsDbContext _context;
    private readonly IQrCodeService _qrCodeService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReceptionController(
        VmsDbContext context,
        IQrCodeService qrCodeService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _qrCodeService = qrCodeService;
        _userManager = userManager;
    }

    [RequirePermission("Visitor.ValidateQR")]
    [HttpGet]
    public IActionResult Index()
    {
        return View(new ReceptionLookupViewModel());
    }

    [RequirePermission("Visitor.ValidateQR")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ValidateQr( ReceptionLookupViewModel model)
    {
        if (!ModelState.IsValid)
            return View("Index", model);

        var input = model.Token.Trim();

        var tokenHash =
            _qrCodeService.ComputeTokenHash(input);

        var visit = await _context.VisitRequests
            .Include(x => x.Department)
            .Include(x => x.VisitVisitors)
                .ThenInclude(x => x.Visitor)
            .Include(x => x.AccessLogs)
            .FirstOrDefaultAsync(x =>
                x.QRTokenHash == tokenHash||
                x.VisitReference == input);

        if (visit == null)
        {
            ModelState.AddModelError(
                string.Empty,
                "Invalid QR code.");

            return View("Index", model);
        }

        var host = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == visit.HostUserId);

        var now = DateTime.Now;

        var isWithinValidPeriod =
            now >= visit.VisitFromDateTime &&
            now <= visit.VisitToDateTime;

        var visitorModels =
            new List<ReceptionVisitorViewModel>();

        foreach (var visitVisitor in visit.VisitVisitors)
        {
            var visitorLogs = visit.AccessLogs
                .Where(x =>
                    x.VisitVisitorId ==
                    visitVisitor.VisitVisitorId)
                .OrderByDescending(x => x.EntryTime)
                .ToList();

            var openLog = visitorLogs
                .FirstOrDefault(x =>
                    x.ExitTime == null);

            var lastCompletedLog = visitorLogs
                .FirstOrDefault(x =>
                    x.ExitTime != null);

            visitorModels.Add(
                new ReceptionVisitorViewModel
                {
                    VisitVisitorId =
                        visitVisitor.VisitVisitorId,

                    VisitorId =
                        visitVisitor.VisitorId,

                    FullName =
                        visitVisitor.Visitor.FullName,

                    IdNumber =
                        visitVisitor.Visitor.IdNumber,

                    CompanyName =
                        visitVisitor.Visitor.CompanyName,

                    Designation =
                        visitVisitor.Visitor.Designation,

                    IsCurrentlyInside =
                        openLog != null,

                    LastCheckInTime =
                        openLog?.EntryTime
                        ?? visitorLogs
                            .FirstOrDefault()
                            ?.EntryTime,

                    LastCheckOutTime =
                        lastCompletedLog?.ExitTime
                });
        }

        var result = new ReceptionVisitViewModel
        {
            VisitRequestId =
                visit.VisitRequestId,

            VisitReference =
                visit.VisitReference,

            VisitFromDateTime =
                visit.VisitFromDateTime,

            VisitToDateTime =
                visit.VisitToDateTime,

            Purpose =
                visit.Purpose,

            MeetingLocation =
                visit.MeetingLocation,

            DepartmentName =
                visit.Department?.DepartmentName,

            HostName =
                host?.FullName ?? "Unknown",

            IsWithinValidPeriod =
                isWithinValidPeriod,

            Visitors =
                visitorModels
        };

        return View("VisitDetails", result);
    }

    [RequirePermission("Visitor.CheckIn")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckIn(long visitRequestId, long visitVisitorId)
    {
        var visit = await _context.VisitRequests
            .Include(x => x.VisitVisitors)
            .FirstOrDefaultAsync(x =>
                x.VisitRequestId == visitRequestId);

        if (visit == null)
            return NotFound();

        var visitVisitor = visit.VisitVisitors
            .FirstOrDefault(x =>
                x.VisitVisitorId == visitVisitorId);

        if (visitVisitor == null)
            return NotFound();

        var now = DateTime.Now;

        if (now < visit.VisitFromDateTime ||
            now > visit.VisitToDateTime)
        {
            TempData["ErrorMessage"] =
                "This visit request is outside the approved access period.";

            return RedirectToAction(nameof(Index));
        }

        var existingOpenLog = await _context.VisitAccessLogs
            .AnyAsync(x =>
                x.VisitRequestId == visitRequestId &&
                x.VisitVisitorId == visitVisitorId &&
                x.ExitTime == null);

        if (existingOpenLog)
        {
            TempData["ErrorMessage"] =
                "This visitor is already checked in.";

            return RedirectToAction(nameof(Index));
        }

        var currentUser =
            await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Unauthorized();

        var log = new VisitAccessLog
        {
            VisitRequestId = visitRequestId,
            VisitVisitorId = visitVisitorId,

            EntryTime = now,
            EntryProcessedByUserId = currentUser.Id,

            CreatedDate = DateTime.UtcNow
        };

        _context.VisitAccessLogs.Add(log);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Visitor checked in successfully.";

        return RedirectToAction(
            nameof(VisitDetails),
            new { id = visitRequestId });
    }

    [RequirePermission("Visitor.CheckOut")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckOut(long visitRequestId, long visitVisitorId)
    {
        var visit = await _context.VisitRequests
            .FirstOrDefaultAsync(x =>
                x.VisitRequestId == visitRequestId);

        if (visit == null)
            return NotFound();

        var openLog = await _context.VisitAccessLogs
            .Where(x =>
                x.VisitRequestId == visitRequestId &&
                x.VisitVisitorId == visitVisitorId &&
                x.ExitTime == null)
            .OrderByDescending(x => x.EntryTime)
            .FirstOrDefaultAsync();

        if (openLog == null)
        {
            TempData["ErrorMessage"] =
                "This visitor is not currently checked in.";

            return RedirectToAction(nameof(Index));
        }

        var currentUser =
            await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Unauthorized();

        openLog.ExitTime = DateTime.Now;
        openLog.ExitProcessedByUserId = currentUser.Id;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Visitor checked out successfully.";

        return RedirectToAction(nameof(Index));
    }

    [RequirePermission("Visitor.ValidateQR")]
    [HttpGet]
    public async Task<IActionResult> VisitDetails(long id)
    {
        var visit = await _context.VisitRequests
            .Include(x => x.Department)
            .Include(x => x.VisitVisitors)
                .ThenInclude(x => x.Visitor)
            .Include(x => x.AccessLogs)
            .FirstOrDefaultAsync(x => x.VisitRequestId == id);

        if (visit == null)
            return NotFound();

        var host = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == visit.HostUserId);

        var now = DateTime.Now;

        var model = new ReceptionVisitViewModel
        {
            VisitRequestId = visit.VisitRequestId,
            VisitReference = visit.VisitReference,
            VisitFromDateTime = visit.VisitFromDateTime,
            VisitToDateTime = visit.VisitToDateTime,
            Purpose = visit.Purpose,
            MeetingLocation = visit.MeetingLocation,
            DepartmentName = visit.Department?.DepartmentName,
            HostName = host?.FullName ?? "Unknown",

            IsWithinValidPeriod =
                now >= visit.VisitFromDateTime &&
                now <= visit.VisitToDateTime,

            Visitors = visit.VisitVisitors.Select(vv =>
            {
                var logs = visit.AccessLogs
                    .Where(x => x.VisitVisitorId == vv.VisitVisitorId)
                    .OrderByDescending(x => x.EntryTime)
                    .ToList();

                var openLog = logs.FirstOrDefault(x => x.ExitTime == null);
                var lastCompletedLog = logs.FirstOrDefault(x => x.ExitTime != null);

                return new ReceptionVisitorViewModel
                {
                    VisitVisitorId = vv.VisitVisitorId,
                    VisitorId = vv.VisitorId,
                    FullName = vv.Visitor.FullName,
                    IdNumber = vv.Visitor.IdNumber,
                    CompanyName = vv.Visitor.CompanyName,
                    Designation = vv.Visitor.Designation,

                    IsCurrentlyInside = openLog != null,

                    LastCheckInTime =
                        openLog?.EntryTime ??
                        logs.FirstOrDefault()?.EntryTime,

                    LastCheckOutTime =
                        lastCompletedLog?.ExitTime
                };
            }).ToList()
        };

        return View(model);
    }

    [RequirePermission("Visitor.ValidateQR")]
    [HttpGet]
    public async Task<IActionResult> Visitors(string? filter)
    {
        var todayStart = DateTime.Today;
        var tomorrowStart = todayStart.AddDays(1);

        var query = _context.VisitVisitors
            .AsNoTracking()
            .Include(x => x.Visitor)
            .Include(x => x.VisitRequest)
            .Where(x =>
                x.VisitRequest.VisitFromDateTime < tomorrowStart &&
                x.VisitRequest.VisitToDateTime >= todayStart &&
                (
                    x.VisitRequest.Status == VisitStatus.Approved ||
                    x.VisitRequest.Status == VisitStatus.ReadyForVisit
                ));

        var visitVisitors = await query
            .OrderBy(x => x.VisitRequest.VisitFromDateTime)
            .ToListAsync();

        var visitVisitorIds = visitVisitors
            .Select(x => x.VisitVisitorId)
            .ToList();

        var logs = await _context.VisitAccessLogs
            .AsNoTracking()
            .Where(x => visitVisitorIds.Contains(x.VisitVisitorId))
            .OrderByDescending(x => x.EntryTime)
            .ToListAsync();

        var result = visitVisitors.Select(vv =>
        {
            var visitorLogs = logs
                .Where(x => x.VisitVisitorId == vv.VisitVisitorId)
                .OrderByDescending(x => x.EntryTime)
                .ToList();

            var openLog = visitorLogs
                .FirstOrDefault(x => x.ExitTime == null);

            var lastLog = visitorLogs.FirstOrDefault();

            return new ReceptionVisitorListItemViewModel
            {
                VisitRequestId = vv.VisitRequestId,
                VisitVisitorId = vv.VisitVisitorId,
                VisitorId = vv.VisitorId,

                VisitReference =
                    vv.VisitRequest.VisitReference,

                FullName =
                    vv.Visitor.FullName,

                IdNumber =
                    vv.Visitor.IdNumber,

                CompanyName =
                    vv.Visitor.CompanyName,

                VisitFromDateTime =
                    vv.VisitRequest.VisitFromDateTime,

                VisitToDateTime =
                    vv.VisitRequest.VisitToDateTime,

                IsCurrentlyInside =
                    openLog != null,

                LastCheckInTime =
                    lastLog?.EntryTime,

                LastCheckOutTime =
                    lastLog?.ExitTime
            };
        }).ToList();

        switch (filter?.ToLower())
        {
            case "inside":
                result = result
                    .Where(x => x.IsCurrentlyInside)
                    .ToList();
                break;

            case "checkedout":
                result = result
                    .Where(x =>
                        x.LastCheckOutTime >= todayStart &&
                        x.LastCheckOutTime < tomorrowStart)
                    .ToList();
                break;

            case "notarrived":
                result = result
                    .Where(x =>
                        !x.IsCurrentlyInside &&
                        x.LastCheckInTime == null)
                    .ToList();
                break;

                // "expected" intentionally returns everybody
                // expected during today's visit window.
        }

        ViewBag.Filter = filter;

        return View(result);
    }

    [RequirePermission("Visitor.ValidateQR")]
    [HttpGet]
    public async Task<IActionResult> AccessHistory(
     string? search,
     DateTime? fromDate,
     DateTime? toDate,
     string? status)
    {
        var query = _context.VisitAccessLogs
            .AsNoTracking()
            .Include(x => x.VisitRequest)
            .Include(x => x.VisitVisitor)
                .ThenInclude(x => x.Visitor)
            .AsQueryable();

        // Search
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.VisitVisitor.Visitor.FullName.Contains(search) ||
                x.VisitVisitor.Visitor.IdNumber.Contains(search) ||
                x.VisitRequest.VisitReference.Contains(search));
        }

        // From date
        if (fromDate.HasValue)
        {
            var start = fromDate.Value.Date;

            query = query.Where(x =>
                x.EntryTime >= start);
        }

        // To date - include full selected day
        if (toDate.HasValue)
        {
            var end = toDate.Value.Date.AddDays(1);

            query = query.Where(x =>
                x.EntryTime < end);
        }

        // Status
        switch (status?.ToLower())
        {
            case "inside":
                query = query.Where(x =>
                    x.ExitTime == null);
                break;

            case "completed":
                query = query.Where(x =>
                    x.ExitTime != null);
                break;
        }

        // Load filtered logs first
        var logs = await query
            .OrderByDescending(x => x.EntryTime)
            .ToListAsync();

        // Get all receptionist/security user IDs
        var userIds = logs
            .SelectMany(x => new[]
            {
            x.EntryProcessedByUserId,
            x.ExitProcessedByUserId
            })
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        // Resolve user IDs to full names
        var users = await _context.Users
            .AsNoTracking()
            .Where(x => userIds.Contains(x.Id))
            .ToDictionaryAsync(
                x => x.Id,
                x => x.FullName);

        // Build view model
        var model = logs
            .Select(x => new AccessHistoryViewModel
            {
                VisitReference =
                    x.VisitRequest.VisitReference,

                VisitorName =
                    x.VisitVisitor.Visitor.FullName,

                IdNumber =
                    x.VisitVisitor.Visitor.IdNumber,

                EntryTime =
                    x.EntryTime,

                ExitTime =
                    x.ExitTime,

                EntryProcessedBy =
                    !string.IsNullOrWhiteSpace(
                        x.EntryProcessedByUserId) &&
                    users.TryGetValue(
                        x.EntryProcessedByUserId,
                        out var entryUser)
                            ? entryUser
                            : "-",

                ExitProcessedBy =
                    !string.IsNullOrWhiteSpace(
                        x.ExitProcessedByUserId) &&
                    users.TryGetValue(
                        x.ExitProcessedByUserId,
                        out var exitUser)
                            ? exitUser
                            : null,

                EntryLocation =
                    x.EntryGateOrLocation,

                ExitLocation =
                    x.ExitGateOrLocation
            })
            .ToList();

        ViewBag.Search = search;

        ViewBag.FromDate =
            fromDate?.ToString("yyyy-MM-dd");

        ViewBag.ToDate =
            toDate?.ToString("yyyy-MM-dd");

        ViewBag.Status = status;

        return View(model);
    }
}