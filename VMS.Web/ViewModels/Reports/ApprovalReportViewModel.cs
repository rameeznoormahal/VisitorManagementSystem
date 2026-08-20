namespace VMS.Web.ViewModels.Reports;

public class ApprovalReportViewModel
{
    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public string? Status { get; set; }

    public string? Search { get; set; }

    public List<ApprovalReportItemViewModel> Items { get; set; }
        = new();
}

public class ApprovalReportItemViewModel
{
    public long VisitRequestId { get; set; }

    public string VisitReference { get; set; } = string.Empty;

    public string HostName { get; set; } = string.Empty;

    public string? DepartmentName { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? DecisionBy { get; set; }

    public DateTime? DecisionDate { get; set; }

    public string? DecisionComments { get; set; }

    public DateTime VisitFromDateTime { get; set; }

    public DateTime VisitToDateTime { get; set; }

    public int VisitorCount { get; set; }
}