using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VMS.Domain.Enums;
using VMS.Infrastructure.Data;
using VMS.Web.Models;

namespace VMS.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly VmsDbContext _context;

        public HomeController(
            ILogger<HomeController> logger,
            VmsDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var todayStart = DateTime.Today;
            var tomorrowStart = todayStart.AddDays(1);

            var today = DateOnly.FromDateTime(now);

            ViewBag.TotalRegisteredVisitors =
                await _context.Visitors.CountAsync();

            ViewBag.ActiveVisitors =
                await _context.Visitors.CountAsync(x => x.IsActive);

            ViewBag.ExpiredIdVisitors =
                await _context.Visitors.CountAsync(x =>
                    x.IdExpiryDate < today);

            ViewBag.PendingApprovals =
                await _context.VisitRequests.CountAsync(x =>
                    x.Status == VisitStatus.PendingApproval);

            ViewBag.ExpectedToday =
                await _context.VisitVisitors
                    .Where(x =>
                        x.VisitRequest.VisitFromDateTime < tomorrowStart &&
                        x.VisitRequest.VisitToDateTime >= todayStart &&
                        (
                            x.VisitRequest.Status == VisitStatus.Approved ||
                            x.VisitRequest.Status == VisitStatus.ReadyForVisit
                        ))
                    .CountAsync();

            ViewBag.CurrentlyInside =
                await _context.VisitAccessLogs
                    .Where(x => x.ExitTime == null)
                    .Select(x => x.VisitVisitorId)
                    .Distinct()
                    .CountAsync();

            ViewBag.CheckedOutToday =
                await _context.VisitAccessLogs
                    .Where(x =>
                        x.ExitTime != null &&
                        x.ExitTime >= todayStart &&
                        x.ExitTime < tomorrowStart)
                    .Select(x => x.VisitVisitorId)
                    .Distinct()
                    .CountAsync();

            var expectedVisitorIds =
                await _context.VisitVisitors
                    .Where(x =>
                        x.VisitRequest.VisitFromDateTime < tomorrowStart &&
                        x.VisitRequest.VisitToDateTime >= todayStart &&
                        (
                            x.VisitRequest.Status == VisitStatus.Approved ||
                            x.VisitRequest.Status == VisitStatus.ReadyForVisit
                        ))
                    .Select(x => x.VisitVisitorId)
                    .ToListAsync();

            var arrivedVisitorIds =
                await _context.VisitAccessLogs
                    .Where(x =>
                        expectedVisitorIds.Contains(x.VisitVisitorId) &&
                        x.EntryTime >= todayStart &&
                        x.EntryTime < tomorrowStart)
                    .Select(x => x.VisitVisitorId)
                    .Distinct()
                    .ToListAsync();

            ViewBag.NotYetArrived =
                expectedVisitorIds
                    .Except(arrivedVisitorIds)
                    .Count();
            // =====================================================
            // DASHBOARD CHARTS
            // =====================================================


            // -----------------------------------------------------
            // 1. VISITOR TREND - LAST 7 DAYS
            // -----------------------------------------------------

            var trendStartDate =
                todayStart.AddDays(-6);

            var trendVisitVisitors =
                await _context.VisitVisitors
                    .AsNoTracking()
                    .Where(x =>
                        x.VisitRequest.VisitFromDateTime >= trendStartDate &&
                        x.VisitRequest.VisitFromDateTime < tomorrowStart)
                    .Select(x => new
                    {
                        x.VisitRequest.VisitFromDateTime
                    })
                    .ToListAsync();


            var trendLabels =
                new List<string>();

            var trendValues =
                new List<int>();


            for (var i = 0; i < 7; i++)
            {
                var date =
                    trendStartDate.AddDays(i);

                trendLabels.Add(
                    date.ToString("dd MMM"));

                trendValues.Add(
                    trendVisitVisitors.Count(x =>
                        x.VisitFromDateTime.Date == date.Date));
            }


            ViewBag.VisitorTrendLabels =
                trendLabels;

            ViewBag.VisitorTrendValues =
                trendValues;


            // -----------------------------------------------------
            // 2. VISITS BY DEPARTMENT
            // -----------------------------------------------------

            var departmentVisits =
                await _context.VisitRequests
                    .AsNoTracking()
                    .Include(x => x.Department)
                    .Where(x => x.DepartmentId != null)
                    .GroupBy(x => new
                    {
                        x.DepartmentId,
                        x.Department!.DepartmentName
                    })
                    .Select(x => new
                    {
                        Department =
                            x.Key.DepartmentName,

                        VisitCount =
                            x.Count()
                    })
                    .OrderByDescending(x =>
                        x.VisitCount)
                    .Take(6)
                    .ToListAsync();


            ViewBag.DepartmentLabels =
                departmentVisits
                    .Select(x => x.Department)
                    .ToList();

            ViewBag.DepartmentValues =
                departmentVisits
                    .Select(x => x.VisitCount)
                    .ToList();


            // -----------------------------------------------------
            // 3. VISITOR ID TYPE DISTRIBUTION
            // -----------------------------------------------------

            var visitorDistribution =
                await _context.Visitors
                    .AsNoTracking()
                    .GroupBy(x => x.IdType)
                    .Select(x => new
                    {
                        IdType =
                            x.Key,

                        VisitorCount =
                            x.Count()
                    })
                    .OrderByDescending(x =>
                        x.VisitorCount)
                    .ToListAsync();


            ViewBag.VisitorTypeLabels =
                visitorDistribution
                    .Select(x =>
                        string.IsNullOrWhiteSpace(x.IdType)
                            ? "Other"
                            : x.IdType)
                    .ToList();

            ViewBag.VisitorTypeValues =
                visitorDistribution
                    .Select(x => x.VisitorCount)
                    .ToList();
            ViewBag.TotalVisits =
    await _context.VisitRequests.CountAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId =
                    Activity.Current?.Id ??
                    HttpContext.TraceIdentifier
            });
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult StatusCode(int code)
        {
            ViewBag.StatusCode = code;

            ViewBag.Title = code switch
            {
                403 => "Access Denied",
                404 => "Page Not Found",
                405 => "Action Not Allowed",
                _ => "Something Went Wrong"
            };

            ViewBag.Message = code switch
            {
                403 => "You do not have permission to access this page.",
                404 => "The page you requested could not be found.",
                405 => "This action cannot be accessed using this request method.",
                _ => "The request could not be completed."
            };

            return View("StatusCode");
        }
    }

}