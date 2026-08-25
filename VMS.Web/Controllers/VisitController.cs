using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VMS.Application.Interfaces;
using VMS.Domain.Entities;
using VMS.Domain.Enums;
using VMS.Infrastructure.Data;
using VMS.Infrastructure.Identity;
using VMS.Web.Authorization;
using VMS.Web.ViewModels.Visits;
using Microsoft.AspNetCore.DataProtection;

namespace VMS.Web.Controllers;

[RequirePermission("Visitor.Create")]
public class VisitController : Controller
{
    private readonly VmsDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IQrCodeService _qrCodeService;
    private readonly IDataProtector _qrProtector;

    public VisitController(VmsDbContext context, UserManager<ApplicationUser> userManager,IQrCodeService qrCodeService, IDataProtectionProvider dataProtectionProvider)
    {
        _context = context;
        _userManager = userManager;
        _qrCodeService = qrCodeService;
        _qrProtector = dataProtectionProvider.CreateProtector("VMS.QR.Token");
    }
    [RequirePermission("Visitor.View")]
    [HttpGet]
    public async Task<IActionResult> Index(string? search,string? status)
    {
        var query = _context.VisitRequests
            .AsNoTracking()
            .Include(x => x.Department)
            .Include(x => x.VisitVisitors)
                .ThenInclude(x => x.Visitor)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.VisitReference.Contains(search) ||
                x.Purpose.Contains(search) ||
                (x.Department != null &&
                 x.Department.DepartmentName.Contains(search)) ||
                x.VisitVisitors.Any(v =>
                    v.Visitor.FullName.Contains(search) ||
                    v.Visitor.IdNumber.Contains(search)));
        }

        switch (status?.ToLower())
        {
            case "pending":
                query = query.Where(x =>
                    x.Status == VisitStatus.PendingApproval);
                break;

            case "approved":
                query = query.Where(x =>
                    x.Status == VisitStatus.Approved);
                break;

            case "ready":
                query = query.Where(x =>
                    x.Status == VisitStatus.ReadyForVisit);
                break;

            case "rejected":
                query = query.Where(x =>
                    x.Status == VisitStatus.Rejected);
                break;

            case "checkedin":
                query = query.Where(x =>
                    x.Status == VisitStatus.CheckedIn);
                break;

            case "checkedout":
                query = query.Where(x =>
                    x.Status == VisitStatus.CheckedOut);
                break;
        }

        var visits = await query
            .OrderByDescending(x => x.VisitFromDateTime)
            .ToListAsync();

        ViewBag.Search = search;
        ViewBag.Status = status;

        return View(visits);
    }

    [RequirePermission("Visitor.Create")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new CreateVisitRequestViewModel
        {
            VisitFromDateTime = DateTime.Now,
            VisitToDateTime = DateTime.Now.AddHours(1),
            VisitorCount = 1,

            Visitors = new List<VisitVisitorEntryViewModel>
        {
            new()
        }
        };

        await LoadLookups(model);

        return View(model);
    }

    [RequirePermission("Visitor.Create")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateVisitRequestViewModel model)
    {
        if (model.Visitors == null || model.Visitors.Count == 0)
        {
            ModelState.AddModelError(
                nameof(model.Visitors),
                "At least one visitor is required.");
        }

        if (model.VisitToDateTime <= model.VisitFromDateTime)
        {
            ModelState.AddModelError(
                nameof(model.VisitToDateTime),
                "Visit To date/time must be later than Visit From date/time.");
        }

        // Validate each visitor before saving anything
        for (var i = 0; i < model.Visitors.Count; i++)
        {
            var visitor = model.Visitors[i];

            if (string.IsNullOrWhiteSpace(visitor.IdNumber))
            {
                ModelState.AddModelError(
                    $"Visitors[{i}].IdNumber",
                    "National ID / Passport number is required.");
            }

            if (string.IsNullOrWhiteSpace(visitor.IdType))
            {
                ModelState.AddModelError(
                    $"Visitors[{i}].IdType",
                    "ID type is required.");
            }

            if (string.IsNullOrWhiteSpace(visitor.FullName))
            {
                ModelState.AddModelError(
                    $"Visitors[{i}].FullName",
                    "Visitor name is required.");
            }

            if (string.IsNullOrWhiteSpace(visitor.PhoneNumber))
            {
                ModelState.AddModelError(
                    $"Visitors[{i}].PhoneNumber",
                    "Phone number is required.");
            }

            // ID must remain valid for the visit
            var visitDate =
                DateOnly.FromDateTime(model.VisitFromDateTime);

            if (visitor.IdExpiryDate < visitDate)
            {
                ModelState.AddModelError(
                    $"Visitors[{i}].IdExpiryDate",
                    $"ID for {visitor.FullName} expires before the visit date.");
            }
        }

        // Do not allow the same ID twice in one request
        var duplicateIds = model.Visitors
            .Where(x => !string.IsNullOrWhiteSpace(x.IdNumber))
            .GroupBy(x => x.IdNumber.Trim(),
                StringComparer.OrdinalIgnoreCase)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToList();

        if (duplicateIds.Count > 0)
        {
            ModelState.AddModelError(
                nameof(model.Visitors),
                "The same visitor cannot be added more than once.");
        }

        if (!ModelState.IsValid)
        {
            await LoadLookups(model);
            return View(model);
        }

        var currentUser =
            await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Unauthorized();

        var visitReference =
            $"VIS-{DateTime.UtcNow:yyyyMMddHHmmssfff}";

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            var visit = new VisitRequest
            {
                VisitReference = visitReference,

                HostUserId = model.HostUserId,
                DepartmentId = model.DepartmentId,

                VisitFromDateTime = model.VisitFromDateTime,
                VisitToDateTime = model.VisitToDateTime,

                Purpose = model.Purpose.Trim(),
                MeetingLocation = model.MeetingLocation?.Trim(),
                Notes = model.Notes?.Trim(),

                Status = VisitStatus.PendingApproval,

                CreatedByUserId = currentUser.Id,
                CreatedDate = DateTime.UtcNow
            };

            _context.VisitRequests.Add(visit);

            foreach (var visitorModel in model.Visitors)
            {
                var normalizedId =
                    visitorModel.IdNumber.Trim();

                // Search Visitor Master by ID
                var visitor = await _context.Visitors
                    .FirstOrDefaultAsync(x =>
                        x.IdNumber == normalizedId);

                if (visitor == null)
                {
                    visitor = new Visitor
                    {
                        IdType = visitorModel.IdType.Trim(),
                        IdNumber = normalizedId,
                        IdExpiryDate = visitorModel.IdExpiryDate,

                        FullName = visitorModel.FullName.Trim(),
                        PhoneNumber = visitorModel.PhoneNumber.Trim(),

                        Email = visitorModel.Email?.Trim(),
                        CompanyName = visitorModel.CompanyName?.Trim(),
                        Designation = visitorModel.Designation?.Trim(),
                        Nationality = visitorModel.Nationality?.Trim(),

                        IsActive = true,
                        CreatedDate = DateTime.UtcNow
                    };

                    _context.Visitors.Add(visitor);
                }
                else
                {
                    // Existing visitor:
                    // update current master information from registration.
                    visitor.IdType = visitorModel.IdType.Trim();
                    visitor.IdExpiryDate = visitorModel.IdExpiryDate;
                    visitor.FullName = visitorModel.FullName.Trim();
                    visitor.PhoneNumber = visitorModel.PhoneNumber.Trim();
                    visitor.Email = visitorModel.Email?.Trim();
                    visitor.CompanyName = visitorModel.CompanyName?.Trim();
                    visitor.Designation = visitorModel.Designation?.Trim();
                    visitor.Nationality = visitorModel.Nationality?.Trim();
                    visitor.UpdatedDate = DateTime.UtcNow;
                }

                visit.VisitVisitors.Add(new VisitVisitor
                {
                    Visitor = visitor
                });
            }

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            TempData["SuccessMessage"] =
                $"Visit request {visitReference} created successfully for " +
                $"{model.Visitors.Count} visitor(s).";

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IActionResult> Index()
    {
        var visits = await _context.VisitRequests
                .Include(x => x.VisitVisitors)
                    .ThenInclude(x => x.Visitor)
                .Include(x => x.Department)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

        return View(visits);
    }

    private async Task LoadLookups( CreateVisitRequestViewModel model)
    {
        model.Hosts = await _context.Users
            .Where(x => x.IsActive)
            .OrderBy(x => x.FullName)
            .Select(x => new SelectListItem
            {
                Value = x.Id,
                Text = x.FullName
            })
            .ToListAsync();

        model.Departments = await _context.Departments
            .Where(x => x.IsActive)
            .OrderBy(x => x.DepartmentName)
            .Select(x => new SelectListItem
            {
                Value = x.DepartmentId.ToString(),
                Text = x.DepartmentName
            })
            .ToListAsync();
    }

    [RequirePermission("Visitor.Edit")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateQr(long id)
    {
        var visit = await _context.VisitRequests
            .Include(x => x.VisitVisitors)
                .ThenInclude(x => x.Visitor)
            .FirstOrDefaultAsync(x => x.VisitRequestId == id);

        if (visit == null)
            return NotFound();

        if (visit.Status != VisitStatus.Approved &&
            visit.Status != VisitStatus.ReadyForVisit)
        {
            TempData["ErrorMessage"] =
                "QR code can only be generated for an approved visit request.";

            return RedirectToAction(nameof(Index));
        }

        // Do not generate another QR if this request already has one.
        if (!string.IsNullOrWhiteSpace(visit.QRTokenHash))
        {
            return RedirectToAction(
                nameof(QrPreview),
                new { id = visit.VisitRequestId });
        }

        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Unauthorized();

        var token = _qrCodeService.GenerateToken();

        visit.QRTokenHash =
            _qrCodeService.ComputeTokenHash(token);

        visit.QRTokenProtected =
            _qrProtector.Protect(token);

        visit.QRGeneratedDate =
            DateTime.UtcNow;

        visit.QRGeneratedByUserId =
            currentUser.Id;

        visit.Status = VisitStatus.ReadyForVisit;
        visit.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return RedirectToAction(
            nameof(QrPreview),
            new { id = visit.VisitRequestId });
    }

    [RequirePermission("Visitor.View")]
    [HttpGet]
    public async Task<IActionResult> QrPreview(long id)
    {
        var visit = await _context.VisitRequests
            .Include(x => x.VisitVisitors)
                .ThenInclude(x => x.Visitor)
            .FirstOrDefaultAsync(x =>
                x.VisitRequestId == id);

        if (visit == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(visit.QRTokenProtected))
        {
            TempData["ErrorMessage"] =
                "QR code has not been generated for this visit request.";

            return RedirectToAction(nameof(Index));
        }

        string token;

        try
        {
            token = _qrProtector.Unprotect(
                visit.QRTokenProtected);
        }
        catch
        {
            TempData["ErrorMessage"] =
                "Unable to read the QR code.";

            return RedirectToAction(nameof(Index));
        }

        var pngBytes =
            _qrCodeService.GenerateQrPng(token);

        var model = new QrPreviewViewModel
        {
            VisitRequestId = visit.VisitRequestId,
            VisitReference = visit.VisitReference,

            VisitorName = string.Join(
                ", ",
                visit.VisitVisitors
                    .Select(x => x.Visitor.FullName)),

            VisitFromDateTime = visit.VisitFromDateTime,
            VisitToDateTime = visit.VisitToDateTime,

            QrBase64 =
                Convert.ToBase64String(pngBytes)
        };

        return View(model);
    }

    [RequirePermission("Visitor.Create")]
    [HttpGet]
    public async Task<IActionResult> FindVisitor(string idNumber)
    {
        if (string.IsNullOrWhiteSpace(idNumber))
        {
            return Json(new
            {
                found = false,
                message = "Please enter National ID / Passport number."
            });
        }

        var normalizedId = idNumber.Trim();

        var visitor = await _context.Visitors
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdNumber == normalizedId);

        if (visitor == null)
        {
            return Json(new
            {
                found = false,
                message = "Visitor not found. Please enter visitor details."
            });
        }

        var today = DateOnly.FromDateTime(DateTime.Today);

        var isExpired = visitor.IdExpiryDate < today;

        return Json(new
        {
            found = true,

            visitorId = visitor.VisitorId,
            idType = visitor.IdType,
            idNumber = visitor.IdNumber,
            idExpiryDate = visitor.IdExpiryDate.ToString("yyyy-MM-dd"),

            fullName = visitor.FullName,
            phoneNumber = visitor.PhoneNumber,
            email = visitor.Email,
            companyName = visitor.CompanyName,
            designation = visitor.Designation,
            nationality = visitor.Nationality,

            isActive = visitor.IsActive,
            isExpired,

            message = isExpired
                ? "Visitor found, but the ID has expired."
                : "Existing visitor found."
        });
    }
}