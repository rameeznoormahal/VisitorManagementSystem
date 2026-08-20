namespace VMS.Web.ViewModels.Reports;

public class AccessReportViewModel
{
    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public string? Search { get; set; }

    public string? Status { get; set; }

    public List<AccessReportItemViewModel> Items { get; set; } = new();
}

public class AccessReportItemViewModel
{
    public long VisitorId { get; set; }

    public string VisitorName { get; set; } = string.Empty;

    public string IdNumber { get; set; } = string.Empty;

    public string VisitReference { get; set; } = string.Empty;

    public DateTime EntryTime { get; set; }

    public DateTime? ExitTime { get; set; }

    public string? CompanyName { get; set; }

    public string? DepartmentName { get; set; }

    public TimeSpan? Duration { get; set; }

    public bool IsCurrentlyInside { get; set; }
}