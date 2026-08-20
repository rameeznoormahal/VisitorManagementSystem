namespace VMS.Web.ViewModels.Reports;

public class VisitReportViewModel
{
    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public int? DepartmentId { get; set; }

    public string? Status { get; set; }

    public string? Search { get; set; }

    public List<VisitReportItemViewModel> Items { get; set; } = new();
}

public class VisitReportItemViewModel
{
    public long VisitRequestId { get; set; }

    public string VisitReference { get; set; } = string.Empty;

    public DateTime VisitFromDateTime { get; set; }

    public DateTime VisitToDateTime { get; set; }

    public string HostName { get; set; } = string.Empty;

    public string? DepartmentName { get; set; }

    public string Purpose { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public int VisitorCount { get; set; }

    public DateTime CreatedDate { get; set; }
}