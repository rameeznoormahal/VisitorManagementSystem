using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VMS.Application.Interfaces;
using VMS.Infrastructure.Data;
using VMS.Web.Authorization;
using VMS.Web.Services;
using VMS.Web.ViewModels.Permit;

namespace VMS.Web.Controllers;

public class PermitController : Controller
{
    private readonly VmsDbContext _context;
    private readonly IQrCodeService _qrCodeService;
    private readonly IDataProtector _qrProtector;
    private readonly IVisitPermitPdfService _pdfService;

    public PermitController(
        VmsDbContext context,
        IQrCodeService qrCodeService,
        IDataProtectionProvider dataProtectionProvider,
        IVisitPermitPdfService pdfService)
    {
        _context = context;
        _qrCodeService = qrCodeService;
        _pdfService = pdfService;

        _qrProtector = dataProtectionProvider
            .CreateProtector("VMS.QR.Token");
    }

    [RequirePermission("Visitor.View")]
    [HttpGet]
    public async Task<IActionResult> Preview(long id)
    {
        var visit = await _context.VisitRequests
            .AsNoTracking()
            .Include(x => x.Department)
            .Include(x => x.VisitVisitors)
                .ThenInclude(x => x.Visitor)
            .FirstOrDefaultAsync(x =>
                x.VisitRequestId == id);

        if (visit == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(visit.QRTokenProtected))
        {
            TempData["ErrorMessage"] =
                "QR code has not been generated for this request.";

            return RedirectToAction(
                "Index",
                "Visit");
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
                "Unable to read the QR code for this permit.";

            return RedirectToAction(
                "Index",
                "Visit");
        }

        var host = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == visit.HostUserId);

        var requester = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == visit.CreatedByUserId);

        var qrBytes =
            _qrCodeService.GenerateQrPng(token);

        var model = new VisitPermitViewModel
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

            RequestedBy =
                requester?.FullName ?? "Unknown",

            CreatedDate =
                visit.CreatedDate,

            Status =
                visit.Status.ToString(),

            QRGeneratedDate =
                visit.QRGeneratedDate,

            QrBase64 =
                Convert.ToBase64String(qrBytes),

            Visitors = visit.VisitVisitors
                .Select(x =>
                    new VisitPermitVisitorViewModel
                    {
                        FullName =
                            x.Visitor.FullName,

                        IdType =
                            x.Visitor.IdType,

                        IdNumber =
                            x.Visitor.IdNumber,

                        IdExpiryDate =
                            x.Visitor.IdExpiryDate,

                        Nationality =
                            x.Visitor.Nationality,

                        CompanyName =
                            x.Visitor.CompanyName,

                        Designation =
                            x.Visitor.Designation,

                        PhoneNumber =
                            x.Visitor.PhoneNumber
                    })
                .ToList()
        };

        return View(model);
    }
    
    [RequirePermission("Visitor.View")]
    [HttpGet]
    public async Task<IActionResult> Download(long id)
    {
        var visit = await _context.VisitRequests
            .AsNoTracking()
            .Include(x => x.Department)
            .Include(x => x.VisitVisitors)
                .ThenInclude(x => x.Visitor)
            .FirstOrDefaultAsync(x =>
                x.VisitRequestId == id);

        if (visit == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(visit.QRTokenProtected))
        {
            TempData["ErrorMessage"] =
                "QR code has not been generated for this request.";

            return RedirectToAction(
                "Index",
                "Visit");
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
                "Unable to read QR information.";

            return RedirectToAction(
                "Index",
                "Visit");
        }

        var host = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == visit.HostUserId);

        var requester = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == visit.CreatedByUserId);

        var qrBytes =
            _qrCodeService.GenerateQrPng(token);

        var model = new VisitPermitViewModel
        {
            VisitRequestId = visit.VisitRequestId,
            VisitReference = visit.VisitReference,

            VisitFromDateTime = visit.VisitFromDateTime,
            VisitToDateTime = visit.VisitToDateTime,

            Purpose = visit.Purpose,
            MeetingLocation = visit.MeetingLocation,

            DepartmentName =
                visit.Department?.DepartmentName,

            HostName =
                host?.FullName ?? "Unknown",

            RequestedBy =
                requester?.FullName ?? "Unknown",

            CreatedDate = visit.CreatedDate,
            Status = visit.Status.ToString(),

            QRGeneratedDate =
                visit.QRGeneratedDate,

            QrBase64 =
                Convert.ToBase64String(qrBytes),

            Visitors = visit.VisitVisitors
                .Select(x =>
                    new VisitPermitVisitorViewModel
                    {
                        FullName = x.Visitor.FullName,
                        IdType = x.Visitor.IdType,
                        IdNumber = x.Visitor.IdNumber,
                        IdExpiryDate = x.Visitor.IdExpiryDate,
                        Nationality = x.Visitor.Nationality,
                        CompanyName = x.Visitor.CompanyName,
                        Designation = x.Visitor.Designation,
                        PhoneNumber = x.Visitor.PhoneNumber
                    })
                .ToList()
        };

        var pdfBytes =
            _pdfService.Generate(model);

        var fileName =
            $"{visit.VisitReference}-Permit.pdf";

        return File(
            pdfBytes,
            "application/pdf",
            fileName);
    }
}