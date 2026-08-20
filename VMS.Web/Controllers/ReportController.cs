using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VMS.Domain.Enums;
using VMS.Infrastructure.Data;
using VMS.Web.Authorization;
using VMS.Web.ViewModels.Reports;

namespace VMS.Web.Controllers;

[Authorize]
public class ReportController : Controller
{
    private readonly VmsDbContext _context;

    public ReportController(VmsDbContext context)
    {
        _context = context;
    }

    [RequirePermission("Report.View")]
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [RequirePermission("Report.View")]
    [HttpGet]
    public async Task<IActionResult> Visits(
        DateTime? fromDate,
        DateTime? toDate,
        int? departmentId,
        string? status,
        string? search)
    {
        var query = _context.VisitRequests
            .AsNoTracking()
            .Include(x => x.Department)
            .Include(x => x.VisitVisitors)
            .AsQueryable();

        if (fromDate.HasValue)
        {
            query = query.Where(x =>
                x.VisitFromDateTime >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
        {
            var endDate = toDate.Value.Date.AddDays(1);

            query = query.Where(x =>
                x.VisitFromDateTime < endDate);
        }

        if (departmentId.HasValue)
        {
            query = query.Where(x =>
                x.DepartmentId == departmentId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<VMS.Domain.Enums.VisitStatus>(
                status,
                true,
                out var parsedStatus))
        {
            query = query.Where(x =>
                x.Status == parsedStatus);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.VisitReference.Contains(search) ||
                x.Purpose.Contains(search));
        }

        var visits = await query
            .OrderByDescending(x => x.VisitFromDateTime)
            .ToListAsync();

        var hostIds = visits
            .Select(x => x.HostUserId)
            .Distinct()
            .ToList();

        var hosts = await _context.Users
            .AsNoTracking()
            .Where(x => hostIds.Contains(x.Id))
            .ToDictionaryAsync(
                x => x.Id,
                x => x.FullName);

        var model = new VisitReportViewModel
        {
            FromDate = fromDate,
            ToDate = toDate,
            DepartmentId = departmentId,
            Status = status,
            Search = search,

            Items = visits.Select(x =>
                new VisitReportItemViewModel
                {
                    VisitRequestId = x.VisitRequestId,
                    VisitReference = x.VisitReference,
                    VisitFromDateTime = x.VisitFromDateTime,
                    VisitToDateTime = x.VisitToDateTime,

                    HostName =
                        hosts.TryGetValue(
                            x.HostUserId,
                            out var hostName)
                            ? hostName
                            : "Unknown",

                    DepartmentName =
                        x.Department?.DepartmentName,

                    Purpose = x.Purpose,
                    Status = x.Status.ToString(),
                    VisitorCount = x.VisitVisitors.Count,
                    CreatedDate = x.CreatedDate
                })
                .ToList()
        };

        ViewBag.Departments =
            await _context.Departments
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.DepartmentName)
                .ToListAsync();

        return View(model);
    }

    [RequirePermission("Report.View")]
    [HttpGet]
    public async Task<IActionResult> Visitors(
    string? search,
    string? status,
    DateTime? fromDate,
    DateTime? toDate)
    {
        var visitorQuery = _context.Visitors
            .AsNoTracking()
            .AsQueryable();

        var today = DateOnly.FromDateTime(DateTime.Today);

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            visitorQuery = visitorQuery.Where(x =>
                x.FullName.Contains(search) ||
                x.IdNumber.Contains(search) ||
                (x.CompanyName != null &&
                 x.CompanyName.Contains(search)) ||
                (x.PhoneNumber != null &&
                 x.PhoneNumber.Contains(search)));
        }

        switch (status?.ToLower())
        {
            case "active":
                visitorQuery = visitorQuery.Where(x =>
                    x.IsActive &&
                    x.IdExpiryDate >= today);
                break;

            case "inactive":
                visitorQuery = visitorQuery.Where(x =>
                    !x.IsActive);
                break;

            case "expired":
                visitorQuery = visitorQuery.Where(x =>
                    x.IdExpiryDate < today);
                break;
        }

        var visitors = await visitorQuery
            .OrderBy(x => x.FullName)
            .ToListAsync();

        var visitorIds = visitors
            .Select(x => x.VisitorId)
            .ToList();

        var visitLinksQuery = _context.VisitVisitors
            .AsNoTracking()
            .Include(x => x.VisitRequest)
            .Where(x => visitorIds.Contains(x.VisitorId));

        if (fromDate.HasValue)
        {
            visitLinksQuery = visitLinksQuery.Where(x =>
                x.VisitRequest.VisitFromDateTime >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
        {
            var endDate = toDate.Value.Date.AddDays(1);

            visitLinksQuery = visitLinksQuery.Where(x =>
                x.VisitRequest.VisitFromDateTime < endDate);
        }

        var visitLinks = await visitLinksQuery
            .ToListAsync();

        var model = new VisitorReportViewModel
        {
            Search = search,
            Status = status,
            FromDate = fromDate,
            ToDate = toDate,

            Items = visitors.Select(visitor =>
            {
                var visits = visitLinks
                    .Where(x => x.VisitorId == visitor.VisitorId)
                    .OrderByDescending(x =>
                        x.VisitRequest.VisitFromDateTime)
                    .ToList();

                return new VisitorReportItemViewModel
                {
                    VisitorId = visitor.VisitorId,
                    FullName = visitor.FullName,
                    IdType = visitor.IdType,
                    IdNumber = visitor.IdNumber,
                    IdExpiryDate = visitor.IdExpiryDate,
                    Nationality = visitor.Nationality,
                    CompanyName = visitor.CompanyName,
                    PhoneNumber = visitor.PhoneNumber,
                    IsActive = visitor.IsActive,

                    TotalVisits = visits.Count,

                    LastVisitDate = visits
                        .Select(x => (DateTime?)
                            x.VisitRequest.VisitFromDateTime)
                        .FirstOrDefault()
                };
            })
            .ToList()
        };

        return View(model);
    }

    [RequirePermission("Report.View")]
    [HttpGet]
    public async Task<IActionResult> Access(
    DateTime? fromDate,
    DateTime? toDate,
    string? search,
    string? status)
    {
        var query = _context.VisitAccessLogs
            .AsNoTracking()
            .Include(x => x.VisitVisitor)
                .ThenInclude(x => x.Visitor)
            .Include(x => x.VisitRequest)
                .ThenInclude(x => x.Department)
            .AsQueryable();

        if (fromDate.HasValue)
        {
            query = query.Where(x =>
                x.EntryTime >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
        {
            var endDate = toDate.Value.Date.AddDays(1);

            query = query.Where(x =>
                x.EntryTime < endDate);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.VisitVisitor.Visitor.FullName.Contains(search) ||
                x.VisitVisitor.Visitor.IdNumber.Contains(search) ||
                x.VisitRequest.VisitReference.Contains(search) ||
                (x.VisitVisitor.Visitor.CompanyName != null &&
                 x.VisitVisitor.Visitor.CompanyName.Contains(search)));
        }

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

        var accessLogs = await query
            .OrderByDescending(x => x.EntryTime)
            .ToListAsync();

        var model = new AccessReportViewModel
        {
            FromDate = fromDate,
            ToDate = toDate,
            Search = search,
            Status = status,

            Items = accessLogs.Select(x =>
                new AccessReportItemViewModel
                {
                    VisitorId =
                        x.VisitVisitor.VisitorId,

                    VisitorName =
                        x.VisitVisitor.Visitor.FullName,

                    IdNumber =
                        x.VisitVisitor.Visitor.IdNumber,

                    CompanyName =
                        x.VisitVisitor.Visitor.CompanyName,

                    VisitReference =
                        x.VisitRequest.VisitReference,

                    DepartmentName =
                        x.VisitRequest.Department?.DepartmentName,

                    EntryTime =
                        x.EntryTime,

                    ExitTime =
                        x.ExitTime,

                    Duration =
                        x.ExitTime.HasValue
                            ? x.ExitTime.Value - x.EntryTime
                            : null,

                    IsCurrentlyInside =
                        x.ExitTime == null
                })
                .ToList()
        };

        return View(model);
    }

    [RequirePermission("Report.View")]
    [HttpGet]
    public async Task<IActionResult> CurrentlyInside(
    string? search,
    int? departmentId)
    {
        var query = _context.VisitAccessLogs
            .AsNoTracking()
            .Include(x => x.VisitVisitor)
                .ThenInclude(x => x.Visitor)
            .Include(x => x.VisitRequest)
                .ThenInclude(x => x.Department)
            .Where(x => x.ExitTime == null)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.VisitVisitor.Visitor.FullName.Contains(search) ||
                x.VisitVisitor.Visitor.IdNumber.Contains(search) ||
                x.VisitRequest.VisitReference.Contains(search) ||
                (x.VisitVisitor.Visitor.CompanyName != null &&
                 x.VisitVisitor.Visitor.CompanyName.Contains(search)));
        }

        if (departmentId.HasValue)
        {
            query = query.Where(x =>
                x.VisitRequest.DepartmentId == departmentId.Value);
        }

        var logs = await query
            .OrderBy(x => x.EntryTime)
            .ToListAsync();

        var now = DateTime.Now;

        var model = new CurrentlyInsideReportViewModel
        {
            Search = search,
            DepartmentId = departmentId,

            Items = logs.Select(x =>
                new CurrentlyInsideReportItemViewModel
                {
                    VisitorId =
                        x.VisitVisitor.VisitorId,

                    VisitorName =
                        x.VisitVisitor.Visitor.FullName,

                    IdNumber =
                        x.VisitVisitor.Visitor.IdNumber,

                    CompanyName =
                        x.VisitVisitor.Visitor.CompanyName,

                    VisitReference =
                        x.VisitRequest.VisitReference,

                    DepartmentName =
                        x.VisitRequest.Department?.DepartmentName,

                    EntryTime =
                        x.EntryTime,

                    DurationInside =
                        now - x.EntryTime
                })
                .ToList()
        };

        ViewBag.Departments =
            await _context.Departments
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.DepartmentName)
                .ToListAsync();

        return View(model);
    }

    [RequirePermission("Report.View")]
    [HttpGet]
    public async Task<IActionResult> Approvals(
    DateTime? fromDate,
    DateTime? toDate,
    string? status,
    string? search)
    {
        var query = _context.VisitRequests
            .AsNoTracking()
            .Include(x => x.Department)
            .Include(x => x.VisitVisitors)
            .AsQueryable();

        if (fromDate.HasValue)
        {
            query = query.Where(x =>
                x.CreatedDate >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
        {
            var endDate = toDate.Value.Date.AddDays(1);

            query = query.Where(x =>
                x.CreatedDate < endDate);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            switch (status.ToLower())
            {
                case "pending":
                    query = query.Where(x =>
                        x.Status == VisitStatus.PendingApproval);
                    break;

                case "approved":
                    query = query.Where(x =>
                        x.Status == VisitStatus.Approved ||
                        x.Status == VisitStatus.ReadyForVisit);
                    break;

                case "rejected":
                    query = query.Where(x =>
                        x.Status == VisitStatus.Rejected);
                    break;
            }
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.VisitReference.Contains(search) ||
                x.Purpose.Contains(search));
        }

        var visits = await query
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();

        var userIds = visits
            .SelectMany(x => new[]
            {
            x.HostUserId,
            x.DecisionByUserId
            })
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        var users = await _context.Users
            .AsNoTracking()
            .Where(x => userIds.Contains(x.Id))
            .ToDictionaryAsync(
                x => x.Id,
                x => x.FullName);

        var model = new ApprovalReportViewModel
        {
            FromDate = fromDate,
            ToDate = toDate,
            Status = status,
            Search = search,

            Items = visits.Select(x =>
                new ApprovalReportItemViewModel
                {
                    VisitRequestId =
                        x.VisitRequestId,

                    VisitReference =
                        x.VisitReference,

                    HostName =
                        users.TryGetValue(
                            x.HostUserId,
                            out var hostName)
                            ? hostName
                            : "Unknown",

                    DepartmentName =
                        x.Department?.DepartmentName,

                    Status =
                        x.Status.ToString(),

                    DecisionBy =
                        !string.IsNullOrWhiteSpace(
                            x.DecisionByUserId) &&
                        users.TryGetValue(
                            x.DecisionByUserId,
                            out var decisionUser)
                            ? decisionUser
                            : null,

                    DecisionDate =
                        x.DecisionDate,

                    DecisionComments =
                        x.DecisionComments,

                    VisitFromDateTime =
                        x.VisitFromDateTime,

                    VisitToDateTime =
                        x.VisitToDateTime,

                    VisitorCount =
                        x.VisitVisitors.Count
                })
                .ToList()
        };

        return View(model);
    }

    [RequirePermission("Report.View")]
    [HttpGet]
    public async Task<IActionResult> Departments(
    DateTime? fromDate,
    DateTime? toDate)
    {
        var query = _context.VisitRequests
            .AsNoTracking()
            .Include(x => x.Department)
            .Include(x => x.VisitVisitors)
            .AsQueryable();

        if (fromDate.HasValue)
        {
            query = query.Where(x =>
                x.VisitFromDateTime >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
        {
            var endDate = toDate.Value.Date.AddDays(1);

            query = query.Where(x =>
                x.VisitFromDateTime < endDate);
        }

        var visits = await query.ToListAsync();

        var departments = await _context.Departments
            .AsNoTracking()
            .OrderBy(x => x.DepartmentName)
            .ToListAsync();

        var model = new DepartmentSummaryViewModel
        {
            FromDate = fromDate,
            ToDate = toDate,

            Items = departments.Select(department =>
            {
                var departmentVisits = visits
                    .Where(x =>
                        x.DepartmentId == department.DepartmentId)
                    .ToList();

                return new DepartmentSummaryItemViewModel
                {
                    DepartmentId =
                        department.DepartmentId,

                    DepartmentName =
                        department.DepartmentName,

                    TotalVisits =
                        departmentVisits.Count,

                    TotalVisitors =
                        departmentVisits.Sum(x =>
                            x.VisitVisitors.Count),

                    PendingVisits =
                        departmentVisits.Count(x =>
                            x.Status == VisitStatus.PendingApproval),

                    ApprovedVisits =
                        departmentVisits.Count(x =>
                            x.Status == VisitStatus.Approved ||
                            x.Status == VisitStatus.ReadyForVisit),

                    RejectedVisits =
                        departmentVisits.Count(x =>
                            x.Status == VisitStatus.Rejected),

                    CompletedVisits =
                        departmentVisits.Count(x =>
                            x.Status == VisitStatus.CheckedOut)
                };
            })
            .ToList()
        };

        return View(model);
    }
}