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
    }
}