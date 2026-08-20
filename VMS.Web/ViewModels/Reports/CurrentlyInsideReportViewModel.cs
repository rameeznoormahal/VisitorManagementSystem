namespace VMS.Web.ViewModels.Reports;

public class CurrentlyInsideReportViewModel
{
    public string? Search { get; set; }

    public int? DepartmentId { get; set; }

    public List<CurrentlyInsideReportItemViewModel> Items { get; set; } = new();
}

public class CurrentlyInsideReportItemViewModel
{
    public long VisitorId { get; set; }

    public string VisitorName { get; set; } = string.Empty;

    public string IdNumber { get; set; } = string.Empty;

    public string? CompanyName { get; set; }

    public string VisitReference { get; set; } = string.Empty;

    public string? DepartmentName { get; set; }

    public DateTime EntryTime { get; set; }

    public TimeSpan DurationInside { get; set; }
}