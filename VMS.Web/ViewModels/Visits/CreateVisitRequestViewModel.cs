using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VMS.Web.ViewModels.Visits;

public class CreateVisitRequestViewModel
{
    public string HostUserId { get; set; } = string.Empty;
    public int? DepartmentId { get; set; }

    public DateTime VisitFromDateTime { get; set; }
    public DateTime VisitToDateTime { get; set; }

    public string Purpose { get; set; } = string.Empty;
    public string? MeetingLocation { get; set; }
    public string? Notes { get; set; }

    public int VisitorCount { get; set; } = 1;

    public List<VisitVisitorEntryViewModel> Visitors { get; set; } = new();

    public List<SelectListItem> Hosts { get; set; } = new();
    public List<SelectListItem> Departments { get; set; } = new();
}