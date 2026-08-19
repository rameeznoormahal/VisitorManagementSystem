using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VMS.Infrastructure.Data;

namespace VMS.Web.Controllers;

[Authorize]
public class AdminController : Controller
{
    private readonly VmsDbContext _context;

    public AdminController(VmsDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalUsers =
            await _context.Users.CountAsync();

        ViewBag.ActiveUsers =
            await _context.Users.CountAsync(x => x.IsActive);

        ViewBag.TotalDepartments =
            await _context.Departments.CountAsync();

        ViewBag.TotalGroups =
            await _context.Groups.CountAsync();

        ViewBag.TotalPermissions =
            await _context.Permissions.CountAsync();

        return View();
    }
}